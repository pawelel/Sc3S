﻿using Microsoft.EntityFrameworkCore;
using Sc3S.Data;
using Sc3S.DTO;
using Sc3S.Entities;
using Sc3S.Exceptions;

namespace Sc3S.Services;

public interface IStuffService
{
    //Task ChangeModelOfAsset(int assetId, int modelId);

    //Task<int> CreateAsset(AssetCreateDto assetCreateDto);

    //Task<(int, int)> CreateAssetCategory(int assetId, int categoryId);

    //Task<(int, int)> CreateAssetDetail(AssetDetailDto assetDetailDto);

    //Task<int> CreateCategory(CategoryCreateDto categoryCreateDto);

    //Task<int> CreateDetail(DetailCreateDto detailCreateDto);

    //Task<int> CreateDevice(DeviceCreateDto deviceCreateDto);

    //Task<int> CreateModel(int deviceId, ModelCreateDto modelCreateDto);

    //Task<(int, int)> CreateModelParameter(ModelParameterDto modelParameterDto);

    //Task<int> CreateParameter(ParameterCreateDto parameterCreateDto);

    //Task DeleteAsset(int assetId);

    //Task DeleteAssetCategory(int assetId, int categoryId);

    //Task DeleteAssetDetail(int assetId, int detailId);

    //Task DeleteCategory(int categoryId);

    //Task DeleteDetail(int detailId);

    //Task DeleteDevice(int deviceId);

    //Task DeleteModel(int modelId);

    //Task DeleteModelParameter(int modelId, int parameterId);

    //Task DeleteParameter(int parameterId);

    //Task<AssetDto> GetAssetById(int assetId);

    Task<IEnumerable<AssetDisplayDto>> GetAssetDisplays();

    //Task<IEnumerable<AssetDto>> GetAssets();

    //Task<IEnumerable<CategoryDto>> GetCategories();

    //Task<IEnumerable<CategoryWithAssetsDto>> GetCategoriesWithAssets();

    //Task<IEnumerable<DeviceWithAssetsDto>> GetDevicesWithAssets();
    //Task<DeviceWithAssetsDto> GetDeviceWithAssets(int deviceId);
    //Task<CategoryDto> GetCategoryById(int categoryId);

    //Task<CategoryWithAssetsDto> GetCategoryByIdWithAssets(int categoryId);

    //Task<DetailDto> GetDetailById(int detailId);

    //Task<IEnumerable<DetailDto>> GetDetails();

    //Task<IEnumerable<DetailWithAssetsDto>> GetDetailsWithAssets();

    //Task<DeviceDto> GetDeviceById(int deviceId);

    //Task<IEnumerable<DeviceDto>> GetDevices();

    //Task<IEnumerable<DeviceDto>> GetDevicesWithModels();

    //Task<ModelDto> GetModelById(int modelId);

    //Task<IEnumerable<ModelDto>> GetModels();

    //Task<IEnumerable<ModelDto>> GetModelsWithAssets();

    //Task<ParameterDto> GetParameterById(int parameterId);

    //Task<IEnumerable<ParameterDto>> GetParameters();

    //Task<IEnumerable<ParameterWithModelsDto>> GetParametersWithModels();

    //Task MarkDeleteAsset(int assetId);

    //Task MarkDeleteAssetCategory(int assetId, int categoryId);

    //Task MarkDeleteAssetDetail(int assetId, int detailId);

    //Task MarkDeleteCategory(int categoryId);

    //Task MarkDeleteDetail(int detailId);

    //Task MarkDeleteDevice(int deviceId);

    //Task MarkDeleteModel(int modelId);

    //Task MarkDeleteModelParameter(int modelId, int parameterId);

    //Task MarkDeleteParameter(int parameterId);

    //Task UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto);

    //Task UpdateAssetCategory(int assetId, int categoryId);

    //Task UpdateAssetDetail(AssetDetailDto assetDetailDto);

    //Task UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto);

    //Task UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto);

    //Task UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto);

    //Task UpdateModel(int modelId, ModelUpdateDto modelUpdateDto);

    //Task UpdateModelParameter(ModelParameterDto modelParameterDto);

    //Task UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto);
    //Task UpdateAssetName(int assetId, string name);
}
public class StuffService : IStuffService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<StuffService> _logger;

    public StuffService(IDbContextFactory<Sc3SContext> factory, ILogger<StuffService> logger)
    {
        _factory = factory;
        _logger = logger;
    }


    public async Task ChangeModelOfAsset(int assetId, int modelId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        _context.Entry(asset).State = EntityState.Modified;

        if (asset.ModelId == modelId)
        {
            _logger.LogWarning("Asset already assigned to model");
            throw new BadRequestException("Asset already has this model");
        }
        var model = await _context.Models.Include(a => a.Assets).FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }

        // delete all asset details
        _context.AssetDetails.RemoveRange(asset.AssetDetails);
        // delete all asset categories
        _context.AssetCategories.RemoveRange(asset.AssetCategories);
        // delete all communicate assets
        _context.CommunicateAssets.RemoveRange(asset.CommunicateAssets);

        // update asset model
        asset.ModelId = modelId;

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Asset with id {AssetId} updated", assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", assetId);


            throw new BadRequestException($"Error updating asset with id {assetId}");
        }
    }

    public async Task<int> CreateAsset(AssetCreateDto assetCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get model
        var model = await _context.Models.Include(m => m.Assets).AsNoTracking().FirstOrDefaultAsync(m => m.ModelId == assetCreateDto.ModelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model marked as deleted");
            throw new BadRequestException("Model marked as deleted");
        }
        // get coordinate
        var coordinate = await _context.Coordinates.AsNoTracking().FirstOrDefaultAsync(c => c.CoordinateId == assetCreateDto.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            throw new NotFoundException("Coordinate not found");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate marked as deleted");
            throw new BadRequestException("Coordinate marked as deleted");
        }

        // validate asset name
        var duplicate = model.Assets.Any(a => a.Name.ToLower().Trim() == assetCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Asset name already exists");
            throw new BadRequestException("Asset name already exists");
        }

        var asset = new Asset
        {
            ModelId = assetCreateDto.ModelId,
            CoordinateId = assetCreateDto.CoordinateId,
            Name = assetCreateDto.Name,
            Description = assetCreateDto.Description,
            Process = assetCreateDto.Process,
            Status = assetCreateDto.Status,
            IsDeleted = false,
            Model = new() { ModelId = assetCreateDto.ModelId },
            Coordinate = new() { CoordinateId = assetCreateDto.CoordinateId }
        };
        // create asset
        _context.Attach(asset);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            _logger.LogInformation("Asset with id {AssetId} created", asset.AssetId);
            return asset.AssetId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            throw new BadRequestException("Error creating asset");
        }
    }

    public async Task<(int, int)> CreateAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await _context.AssetCategories.FirstOrDefaultAsync(a => a.AssetId == assetId && a.CategoryId == categoryId);
        if (assetCategory != null)
            throw new BadRequestException("AssetCategory already exists");
        var category = categoryId < 1 ? null : await _context.Categories.FirstOrDefaultAsync(a => a.CategoryId == categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        var asset = assetId < 1 ? null : await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        assetCategory = new AssetCategory
        {
            AssetId = assetId,
            CategoryId = categoryId,
            IsDeleted = false
        };
        _context.ChangeTracker.Clear();
        _context.AssetCategories.Add(assetCategory);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return (assetId, categoryId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating assetCategory");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<(int, int)> CreateAssetDetail(AssetDetailDto assetDetailDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail != null)
        {
            _logger.LogWarning("AssetDetail already exists");
            throw new BadRequestException("AssetDetail already exists");
        }
        var asset = await _context.Assets.FindAsync(assetDetailDto.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var detail = await _context.Details.FindAsync(assetDetailDto.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        assetDetail = new AssetDetail
        {
            AssetId = assetDetailDto.AssetId,
            DetailId = assetDetailDto.DetailId,
            Value = assetDetailDto.Value,
            IsDeleted = false
        };
        // save changes

        try
        {
            _context.ChangeTracker.Clear();
            _context.AssetDetails.Add(assetDetail);
            await _context.SaveChangesAsync();

            return (assetDetailDto.AssetId, assetDetailDto.DetailId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating assetDetail");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<int> CreateCategory(CategoryCreateDto categoryCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // validate category name
        var duplicate = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == categoryCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Category name already exists");
            throw new BadRequestException("Category name already exists");
        }

        var category = new Category
        {
            Name = categoryCreateDto.Name,
            Description = categoryCreateDto.Description,
            IsDeleted = false
        };
        // create category
        _context.ChangeTracker.Clear();
        _context.Categories.Attach(category);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Category with id {CategoryId} created", category.CategoryId);
            return category.CategoryId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");


            throw new BadRequestException("Error creating category");
        }
    }

    public async Task<int> CreateDetail(DetailCreateDto detailCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // validate detail name
        var duplicate = await _context.Details.AnyAsync(d => d.Name.ToLower().Trim() == detailCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Detail name already exists");
            throw new BadRequestException("Detail name already exists");
        }

        var detail = new Detail
        {
            Name = detailCreateDto.Name,
            Description = detailCreateDto.Description,
            IsDeleted = false
        };
        // create detail
        _context.Details.Attach(detail);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Detail with id {DetailId} created", detail.DetailId);
            return detail.DetailId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating detail");


            throw new BadRequestException("Error creating detail");
        }
    }

    public async Task<int> CreateDevice(DeviceCreateDto deviceCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // validate device name
        var duplicate = await _context.Devices.AnyAsync(d => d.Name.ToLower().Trim() == deviceCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Device name already exists");
            throw new BadRequestException("Device name already exists");
        }

        var device = new Device
        {
            Name = deviceCreateDto.Name,
            Description = deviceCreateDto.Description,
            IsDeleted = false
        };
        // create device
        _context.Devices.Attach(device);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Device with id {DeviceId} created", device.DeviceId);
            return device.DeviceId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");


            throw new BadRequestException("Error creating device");
        }
    }

    public async Task<int> CreateModel(int deviceId, ModelCreateDto modelCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get device
        var device = await _context.Devices.Include(d => d.Models).AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }

        // validate model name
        var duplicate = device.Models.Any(m => m.Name.ToLower().Trim() == modelCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Model name already exists");
            throw new BadRequestException("Model name already exists");
        }
        // check if device is marked as deleted

        var model = new Model
        {
            DeviceId = deviceId,
            Device = new() { DeviceId = deviceId },
            Name = modelCreateDto.Name,
            Description = modelCreateDto.Description,
            IsDeleted = false
        };
        // create model
        _context.Attach(model);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Model with id {ModelId} created", model.ModelId);
            return model.ModelId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");


            throw new BadRequestException("Error creating model");
        }
    }

    public async Task<(int, int)> CreateModelParameter(ModelParameterDto modelParameterDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        if (modelParameter != null)
        {
            throw new BadRequestException("ModelParameter already exists");
        }
        var model = await _context.Models.FindAsync(modelParameterDto.ModelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        var parameter = await _context.Parameters.FindAsync(modelParameterDto.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        modelParameter = new ModelParameter
        {
            ModelId = modelParameterDto.ModelId,
            ParameterId = modelParameterDto.ParameterId,
            Value = modelParameterDto.Value,
            IsDeleted = false
        };
        // save changes

        try
        {
            _context.ModelParameters.Attach(modelParameter);
            await _context.SaveChangesAsync();

            return (modelParameter.ModelId, modelParameter.ParameterId);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating modelParameter");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task<int> CreateParameter(ParameterCreateDto parameterCreateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // validate parameter name
        var duplicate = await _context.Parameters.AnyAsync(p => p.Name.ToLower().Trim() == parameterCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Parameter name already exists");
            throw new BadRequestException("Parameter name already exists");
        }
        var parameter = new Parameter
        {

            Name = parameterCreateDto.Name,
            Description = parameterCreateDto.Description,
            IsDeleted = false
        };
        // create parameter
        _context.Parameters.Attach(parameter);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Parameter with id {ParameterId} created", parameter.ParameterId);
            return parameter.ParameterId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");


            throw new BadRequestException("Error creating parameter");
        }
    }

    public async Task DeleteAsset(int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get asset
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        // check if asset is marked as deleted
        if (asset.IsDeleted == false)
        {
            _logger.LogWarning("Asset is not marked as deleted");
            throw new BadRequestException("Asset is not marked as deleted");
        }
        // delete asset
        _context.Assets.Remove(asset);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Asset with id {AssetId} deleted", asset.AssetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset");


            throw new BadRequestException("Error deleting asset");
        }
    }

    public async Task DeleteAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        // get assetCategory
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            throw new NotFoundException("AssetCategory not found");
        }
        if (assetCategory.IsDeleted == false)
        {
            _logger.LogWarning("AssetCategory is not marked as deleted");
            throw new BadRequestException("AssetCategory is not marked as deleted");
        }

        _context.AssetCategories.Remove(assetCategory);


        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetCategory deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetCategory");

            throw new BadRequestException("Error deleting assetCategory");
        }
    }

    public async Task DeleteAssetDetail(int assetId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetId, detailId);

        // get assetDetail
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            throw new NotFoundException("AssetDetail not found");
        }
        if (assetDetail.IsDeleted == false)
        {
            _logger.LogWarning("AssetDetail is not marked as deleted");
            throw new BadRequestException("AssetDetail is not marked as deleted");
        }



        try
        {
            _context.AssetDetails.Remove(assetDetail);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetDetail deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetDetail");

            throw new BadRequestException("Error deleting assetDetail");
        }
    }

    public async Task DeleteCategory(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get category
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category is marked as deleted
        if (category.IsDeleted == false)
        {
            _logger.LogWarning("Category is not marked as deleted");
            throw new BadRequestException("Category is not marked as deleted");
        }
        // delete category
        _context.Categories.Remove(category);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Category with id {CategoryId} deleted", category.CategoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");


            throw new BadRequestException("Error deleting category");
        }
    }

    public async Task DeleteDetail(int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get detail
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail is marked as deleted
        if (detail.IsDeleted == false)
        {
            _logger.LogWarning("Detail is not marked as deleted");
            throw new BadRequestException("Detail is not marked as deleted");
        }
        // delete detail
        _context.Details.Remove(detail);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");


            throw new BadRequestException("Error deleting detail");
        }
    }

    public async Task DeleteDevice(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get device
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device is marked as deleted
        if (device.IsDeleted == false)
        {
            _logger.LogWarning("Device is not marked as deleted");
            throw new BadRequestException("Device is not marked as deleted");
        }
        // delete device
        _context.Devices.Remove(device);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Device with id {DeviceId} deleted", device.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device");


            throw new BadRequestException("Error deleting device");
        }
    }

    public async Task DeleteModel(int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get model
        var model = await _context.Models.FindAsync(modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model is marked as deleted
        if (model.IsDeleted == false)
        {
            _logger.LogWarning("Model is not marked as deleted");
            throw new BadRequestException("Model is not marked as deleted");
        }
        // delete model
        _context.Models.Remove(model);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model");


            throw new BadRequestException("Error deleting model");
        }
    }

    public async Task DeleteModelParameter(int modelId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelId, parameterId);

        // get modelParameter
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            throw new NotFoundException("ModelParameter not found");
        }
        if (modelParameter.IsDeleted)
        {

            try
            {
                _context.ModelParameters.Remove(modelParameter);
                // save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("ModelParameter deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting modelParameter");

                throw new BadRequestException("Error deleting modelParameter");
            }
        }
        _logger.LogWarning("ModelParameter is not marked as deleted");
        throw new BadRequestException("ModelParameter is not marked as deleted");
    }

    public async Task DeleteParameter(int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get parameter
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter is marked as deleted
        if (parameter.IsDeleted == false)
        {
            _logger.LogWarning("Parameter is not marked as deleted");
            throw new BadRequestException("Parameter is not marked as deleted");
        }
        // delete parameter
        _context.Parameters.Remove(parameter);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter");


            throw new BadRequestException("Error deleting parameter");
        }
    }

    public async Task<AssetDto> GetAssetById(int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get asset
        var asset = await _context.Assets.AsNoTracking().Select(a => new AssetDto
        {
            AssetId = a.AssetId,
            Name = a.Name,
            Description = a.Description,
            IsDeleted = a.IsDeleted,
            UserId = a.UpdatedBy,
            Status = a.Status,
            CoordinateId = a.CoordinateId,
            ModelId = a.ModelId,
            Process = a.Process
        }).FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        // return asset
        _logger.LogInformation("Asset with id {AssetId} found", asset.AssetId);
        return asset;
    }

    public async Task<IEnumerable<AssetDisplayDto>> GetAssetDisplays()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get asset
        var query = await _context.Assets
            .AsNoTracking()
            .Select(a => new AssetDisplayDto
            {
                Name = a.Name,
                Description = a.Description,
                AssetId = a.AssetId,
                AreaId = a.Coordinate.Space.AreaId,
                SpaceId = a.Coordinate.SpaceId,
                CoordinateId = a.CoordinateId,
                PlantId = a.Coordinate.Space.Area.PlantId,
                AreaName = a.Coordinate.Space.Area.Name,
                SpaceName = a.Coordinate.Space.Name,
                CoordinateName = a.Coordinate.Name,
                Status = a.Status,
                IsDeleted = a.IsDeleted,
                UserId = a.UpdatedBy,
                Categories = a.AssetCategories.Select(ac => new AssetCategoryDisplayDto
                {
                    Name = ac.Category.Name,
                    AssetId = ac.AssetId,
                    UserId = ac.UpdatedBy,
                    CategoryId = ac.CategoryId,
                    Description = ac.Category.Description,
                    IsDeleted = ac.IsDeleted
                }).ToList(),
                Details = a.AssetDetails.Select(ad => new AssetDetailDisplayDto
                {
                    Name = ad.Detail.Name,
                    Value = ad.Value,
                    DetailId = ad.DetailId,
                    UserId = ad.UpdatedBy,
                    AssetId = ad.AssetId,
                    Description = ad.Detail.Description,
                    IsDeleted = ad.IsDeleted
                }).ToList(),
                Parameters = a.Model.ModelParameters.Select(mp => new ModelParameterDisplayDto
                {
                    Name = mp.Parameter.Name,
                    Value = mp.Value,
                    ParameterId = mp.ParameterId,
                    UserId = mp.UpdatedBy,
                    ModelId = mp.ModelId,
                    Description = mp.Parameter.Description,
                    IsDeleted = mp.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No assets found");
            throw new NotFoundException("No assets found");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return query;
    }

    public async Task<IEnumerable<AssetDto>> GetAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get assets
        var assets = await _context.Assets
            .AsNoTracking()
            .Select(a => new AssetDto
            {
                AssetId = a.AssetId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UserId = a.UpdatedBy,
                Status = a.Status,
                CoordinateId = a.CoordinateId,
                ModelId = a.ModelId,
                Process = a.Process
            }).ToListAsync();
        if (assets is null)
        {
            _logger.LogWarning("No assets found");
            throw new NotFoundException("No assets found");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return assets;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get categories
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto
            {
                Name = c.Name,
                Description = c.Description,
                CategoryId = c.CategoryId,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No categories found");
            throw new NotFoundException("No categories found");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return query;
    }

    public async Task<IEnumerable<CategoryWithAssetsDto>> GetCategoriesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get categories
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryWithAssetsDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Assets = c.AssetCategories.Select(ac => new AssetDto
                {
                    Name = ac.Asset.Name,
                    Description = ac.Asset.Description,
                    AssetId = ac.AssetId,
                    Status = ac.Asset.Status,
                    UserId = ac.UpdatedBy,
                    Process = ac.Asset.Process,
                    IsDeleted = ac.Asset.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No categories found");
            throw new NotFoundException("No categories found");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return query;
    }

    public async Task<CategoryDto> GetCategoryById(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get category
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto
            {
                Name = c.Name,
                UserId = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Description = c.Description,
                CategoryId = c.CategoryId
            }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        if (query == null)
        {
            _logger.LogWarning("No category found");
            throw new NotFoundException("No category found");
        }
        // return category
        _logger.LogInformation("Category found");
        return query;
    }

    public async Task<CategoryWithAssetsDto> GetCategoryByIdWithAssets(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get category
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryWithAssetsDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Assets = c.AssetCategories.Select(ac => new AssetDto
                {
                    Name = ac.Asset.Name,
                    Description = ac.Asset.Description,
                    AssetId = ac.AssetId,
                    UserId = ac.UpdatedBy,
                    IsDeleted = ac.IsDeleted,
                    Status = ac.Asset.Status,
                    Process = ac.Asset.Process
                }).ToList()
            }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        if (query == null)
        {
            _logger.LogWarning("No category found");
            throw new NotFoundException("No category found");
        }
        // return category
        _logger.LogInformation("Category found");
        return query;
    }

    public async Task<DetailDto> GetDetailById(int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get detail
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailDto
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UserId = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).FirstOrDefaultAsync(d => d.DetailId == detailId);
        if (query == null)
        {
            _logger.LogWarning("No detail found");
            throw new NotFoundException("No detail found");
        }
        // return detail
        _logger.LogInformation("Detail found");
        return query;
    }

    public async Task<IEnumerable<DetailDto>> GetDetails()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get details
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailDto
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UserId = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No details found");
            throw new NotFoundException("No details found");
        }

        // return details
        _logger.LogInformation("Details found");
        return query;
    }

    public async Task<IEnumerable<DetailWithAssetsDto>> GetDetailsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get details
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailWithAssetsDto
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UserId = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                Assets = d.AssetDetails.Select(ad => new AssetDto
                {
                    Name = ad.Asset.Name,
                    Description = ad.Asset.Description,
                    AssetId = ad.AssetId,
                    UserId = ad.UpdatedBy,
                    Status = ad.Asset.Status,
                    Process = ad.Asset.Process,
                    IsDeleted = ad.Asset.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No details found");
            throw new NotFoundException("No details found");
        }
        // return details
        _logger.LogInformation("Details found");
        return query;
    }

    public async Task<DeviceDto> GetDeviceById(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get device
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceDto
            {
                DeviceId = d.DeviceId,
                UserId = d.UpdatedBy,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (query == null)
        {
            _logger.LogWarning("No device found");
            throw new NotFoundException("No device found");
        }
        // return device
        _logger.LogInformation("Device found");
        return query;
    }

    public async Task<IEnumerable<DeviceDto>> GetDevices()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceDto
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UserId = d.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            throw new NotFoundException("No devices found");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return query;
    }

    public async Task<IEnumerable<DeviceDto>> GetDevicesWithModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceDto
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UserId = d.UpdatedBy,
                Models = d.Models.Select(m => new ModelDto
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UserId = m.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            throw new NotFoundException("No devices found");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return query;
    }

    public async Task<IEnumerable<DeviceWithAssetsDto>> GetDevicesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceWithAssetsDto
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UserId = d.UpdatedBy,
                Assets = d.Models.SelectMany(m => m.Assets).Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            throw new NotFoundException("No devices found");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return query;
    }
    public async Task<DeviceWithAssetsDto> GetDeviceWithAssets(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get device
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceWithAssetsDto
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UserId = d.UpdatedBy,
                Assets = d.Models.SelectMany(m => m.Assets).Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (query is null)
        {
            _logger.LogWarning("No device found");
            throw new NotFoundException("No device found");
        }
        // return device
        _logger.LogInformation("Device found");
        return query;
    }

    public async Task<ModelDto> GetModelById(int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelDto
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UserId = m.UpdatedBy
            }).FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (query == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // return model
        _logger.LogInformation("Model found");
        return query;
    }

    public async Task<IEnumerable<ModelDto>> GetModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get models
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelDto
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UserId = m.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No models found");
            throw new NotFoundException("No models found");
        }
        // return models
        _logger.LogInformation("Models found");
        return query;
    }

    public async Task<IEnumerable<ModelDto>> GetModelsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get models
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelDto
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UserId = m.UpdatedBy,
                Assets = m.Assets.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UserId = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No models found");
            throw new NotFoundException("No models found");
        }
        // return models
        _logger.LogInformation("Models found");
        return query;
    }

    public async Task<ParameterDto> GetParameterById(int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get parameter
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(m => new ParameterDto
            {
                ParameterId = m.ParameterId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UserId = m.UpdatedBy
            }).FirstOrDefaultAsync(m => m.ParameterId == parameterId);
        if (query == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // return parameter
        _logger.LogInformation("Parameter found");
        return query;
    }

    public async Task<IEnumerable<ParameterDto>> GetParameters()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get parameters
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(m => new ParameterDto
            {
                ParameterId = m.ParameterId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UserId = m.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No parameters found");
            throw new NotFoundException("No parameters found");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return query;
    }

    public async Task<IEnumerable<ParameterWithModelsDto>> GetParametersWithModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get parameters
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(p => new ParameterWithModelsDto
            {
                ParameterId = p.ParameterId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UserId = p.UpdatedBy,
                Models = p.ModelParameters.Select(mp => new ModelDto
                {
                    ModelId = mp.ModelId,
                    Name = mp.Model.Name,
                    Description = mp.Model.Description,
                    IsDeleted = mp.Model.IsDeleted,
                    UserId = mp.Model.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No parameters found");
            throw new NotFoundException("No parameters found");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return query;
    }

    public async Task MarkDeleteAsset(int assetId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get asset
        var asset = await _context.Assets
            .FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        // check if asset is already deleted
        if (asset.IsDeleted)
        {
            _logger.LogWarning("Asset is already deleted");
            throw new BadRequestException("Asset is already deleted");
        }
        // mark asset as deleted
        asset.IsDeleted = true;
        // update asset
        _context.Update(asset);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Asset marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking asset as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking asset as deleted");
        }
    }

    public async Task MarkDeleteAssetCategory(int assetId, int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get assetCategory
        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            throw new NotFoundException("AssetCategory not found");
        }
        // check if assetCategory is already deleted
        if (assetCategory.IsDeleted)
        {
            _logger.LogWarning("AssetCategory already deleted");
            throw new BadRequestException("AssetCategory already deleted");
        }
        assetCategory.IsDeleted = true;
        _context.Update(assetCategory);


        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetCategory marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetCategory as deleted");

            throw new BadRequestException("Error marking assetCategory as deleted");
        }
    }

    public async Task MarkDeleteAssetDetail(int assetId, int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get assetDetail
        var assetDetail = await _context.AssetDetails.FindAsync(assetId, detailId);
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            throw new NotFoundException("AssetDetail not found");
        }
        // check if assetDetail is already deleted
        if (assetDetail.IsDeleted)
        {
            _logger.LogWarning("AssetDetail already deleted");
            throw new BadRequestException("AssetDetail already deleted");
        }
        assetDetail.IsDeleted = true;
        _context.Update(assetDetail);


        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetDetail marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetDetail as deleted");

            throw new BadRequestException("Error marking assetDetail as deleted");
        }
    }

    public async Task MarkDeleteCategory(int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get category
        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.CategoryId == categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category is already deleted
        if (category.IsDeleted)
        {
            _logger.LogWarning("Category is already deleted");
            throw new BadRequestException("Category is already deleted");
        }
        // mark category as deleted
        category.IsDeleted = true;
        // update category
        _context.Update(category);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Category marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking category as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking category as deleted");
        }
    }

    public async Task MarkDeleteDetail(int detailId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get detail
        var detail = await _context.Details
            .FirstOrDefaultAsync(m => m.DetailId == detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail is already deleted
        if (detail.IsDeleted)
        {
            _logger.LogWarning("Detail is already deleted");
            throw new BadRequestException("Detail is already deleted");
        }
        // mark detail as deleted
        detail.IsDeleted = true;
        // update detail
        _context.Update(detail);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Detail marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking detail as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking detail as deleted");
        }
    }

    public async Task MarkDeleteDevice(int deviceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get device
        var device = await _context.Devices
            .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device is already deleted
        if (device.IsDeleted)
        {
            _logger.LogWarning("Device is already deleted");
            throw new BadRequestException("Device is already deleted");
        }
        // mark device as deleted
        device.IsDeleted = true;
        // update device
        _context.Update(device);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Device marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking device as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking device as deleted");
        }
    }

    public async Task MarkDeleteModel(int modelId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get model
        var model = await _context.Models
            .FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model is already deleted
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model is already deleted");
            throw new BadRequestException("Model is already deleted");
        }
        // mark model as deleted
        model.IsDeleted = true;
        // update model
        _context.Update(model);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Model marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking model as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking model as deleted");
        }
    }

    public async Task MarkDeleteModelParameter(int modelId, int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        var modelParameter = await _context.ModelParameters.FindAsync(modelId, parameterId);
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            throw new NotFoundException("ModelParameter not found");
        }
        if (modelParameter.IsDeleted)
        {
            _logger.LogWarning("ModelParameter already deleted");
            throw new BadRequestException("ModelParameter already deleted");
        }
        modelParameter.IsDeleted = true;
        _context.Update(modelParameter);


        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("ModelParameter marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking modelParameter as deleted");

            throw new BadRequestException("Error marking modelParameter as deleted");
        }
    }

    public async Task MarkDeleteParameter(int parameterId)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get parameter
        var parameter = await _context.Parameters
            .FirstOrDefaultAsync(m => m.ParameterId == parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter is already deleted
        if (parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter is already deleted");
            throw new BadRequestException("Parameter is already deleted");
        }
        // mark parameter as deleted
        parameter.IsDeleted = true;
        // update parameter
        _context.Update(parameter);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Parameter marked as deleted");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking parameter as deleted");
            // await rollback transaction

            throw new BadRequestException("Error marking parameter as deleted");
        }
    }

    public async Task UpdateAsset(int assetId, AssetUpdateDto assetUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get asset with all related data
        var asset = await _context.Assets.FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }

        // get coordinate
        var coordinateFromDto = asset.CoordinateId == assetUpdateDto.CoordinateId
            ? null
            : await _context.Coordinates.AsNoTracking()
                .FirstOrDefaultAsync(m => m.CoordinateId == assetUpdateDto.CoordinateId);

        // if new coordinate is different from old coordinate
        if (coordinateFromDto is { IsDeleted: false } && asset.CoordinateId != assetUpdateDto.CoordinateId)
        {
            asset.CoordinateId = assetUpdateDto.CoordinateId;
            asset.Coordinate = coordinateFromDto;
            _context.Attach(asset);
        }

        // assign userId to update
        asset.Description = assetUpdateDto.Description;
        asset.IsDeleted = false;
        // update asset
        _context.Update(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Asset updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating asset");
            // await rollback transaction

            throw new BadRequestException("Error updating asset");
        }
    }

    public async Task UpdateAssetName(int assetId, string name)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get asset
        var asset = await _context.Assets.FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }

        var allAssets = await _context.Assets.AsNoTracking().ToListAsync();
        if (allAssets.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim() && a.AssetId != assetId))
        {
            _logger.LogWarning("Asset name already exists");
            throw new BadRequestException("Asset name already exists");
        }
        // assign userId to update
        asset.Name = name;
        // update asset
        _context.Update(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction


            _logger.LogInformation("Asset updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating asset");
            // await rollback transaction

            throw new BadRequestException("Error updating asset");
        }
    }

    public async Task UpdateAssetCategory(int assetId, int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            throw new NotFoundException("AssetCategory not found");
        }
        if (!assetCategory.IsDeleted)
            throw new BadRequestException("AssetCategory not marked as deleted");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        assetCategory.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating assetCategory");
            throw new BadRequestException("Error while saving changes");
        }
        throw new BadRequestException("AssetCategory not marked as deleted");
    }

    public async Task UpdateAssetDetail(AssetDetailDto assetDetailDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            throw new NotFoundException("AssetDetail not found");
        }
        var asset = await _context.Assets.FindAsync(assetDetailDto.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            throw new NotFoundException("Asset not found");
        }
        var detail = await _context.Details.FindAsync(assetDetailDto.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }

        assetDetail.IsDeleted = false;
        assetDetail.Value = assetDetailDto.Value;

        // save changes

        try
        {
            _context.AssetDetails.Update(assetDetail);
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating assetDetail");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateCategory(int categoryId, CategoryUpdateDto categoryUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get category with all related data
        var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            throw new NotFoundException("Category not found");
        }
        // check if category name from dto is already taken
        var duplicate = await _context.Categories.AnyAsync(a => a.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim());
        if (duplicate || category.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Category name is already taken");
            throw new BadRequestException("Category name is already taken");
        }
        category.Name = categoryUpdateDto.Name;
        // assign userId to update
        category.Description = categoryUpdateDto.Description;
        category.IsDeleted = false;
        // update category
        _context.Update(category);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Category updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating category");
            // await rollback transaction

            throw new BadRequestException("Error updating category");
        }
    }

    public async Task UpdateDetail(int detailId, DetailUpdateDto detailUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get detail
        var detail = await _context.Details.FirstOrDefaultAsync(m => m.DetailId == detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            throw new NotFoundException("Detail not found");
        }
        // check if detail name from dto is already taken
        var duplicate = await _context.Details.AnyAsync(a => a.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim());
        if (duplicate || detail.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Detail name is already taken");
            throw new BadRequestException("Detail name is already taken");
        }
        detail.Name = detailUpdateDto.Name;
        // assign userId to update
        detail.Description = detailUpdateDto.Description;
        detail.IsDeleted = false;
        // update detail
        _context.Update(detail);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Detail updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating detail");
            // await rollback transaction

            throw new BadRequestException("Error updating detail");
        }
    }

    public async Task UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get device
        var device = await _context.Devices.FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            throw new NotFoundException("Device not found");
        }
        // check if device name from dto is already taken
        var duplicate = await _context.Devices.AnyAsync(a => a.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim());
        if (duplicate || device.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Device name is already taken");
            throw new BadRequestException("Device name is already taken");
        }
        device.Name = deviceUpdateDto.Name;
        // assign userId to update
        device.Description = deviceUpdateDto.Description;
        device.IsDeleted = false;
        // update device
        _context.Update(device);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Device updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating device");
            // await rollback transaction

            throw new BadRequestException("Error updating device");
        }
    }

    public async Task UpdateModel(int modelId, ModelUpdateDto modelUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get model
        var model = await _context.Models.FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        // check if model name from dto is already taken
        var duplicate = await _context.Models.AnyAsync(a => a.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim());
        if (duplicate || model.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Model name is already taken");
            throw new BadRequestException("Model name is already taken");
        }
        model.Name = modelUpdateDto.Name;
        // assign userId to update
        model.Description = modelUpdateDto.Description;
        model.IsDeleted = false;
        // update model
        _context.Update(model);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Model updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating model");
            // await rollback transaction

            throw new BadRequestException("Error updating model");
        }
    }

    public async Task UpdateModelParameter(ModelParameterDto modelParameterDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            throw new NotFoundException("ModelParameter not found");
        }
        var model = await _context.Models.FindAsync(modelParameterDto.ModelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            throw new NotFoundException("Model not found");
        }
        var parameter = await _context.Parameters.FindAsync(modelParameterDto.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }

        modelParameter.IsDeleted = false;
        modelParameter.Value = modelParameterDto.Value;
        // save changes

        try
        {
            _context.ModelParameters.Update(modelParameter);
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating modelParameter");
            throw new BadRequestException("Error while saving changes");
        }
    }

    public async Task UpdateParameter(int parameterId, ParameterUpdateDto parameterUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();


        // get parameter
        var parameter = await _context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            throw new NotFoundException("Parameter not found");
        }
        // check if parameter name from dto is already taken
        var duplicate = await _context.Parameters.AnyAsync(a => a.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim());
        if (duplicate || parameter.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Parameter name is already taken");
            throw new BadRequestException("Parameter name is already taken");
        }
        parameter.Name = parameterUpdateDto.Name;
        // assign userId to update
        parameter.Description = parameterUpdateDto.Description;
        parameter.IsDeleted = false;
        // update parameter
        _context.Update(parameter);


        try
        {
            // save changes
            await _context.SaveChangesAsync();
            // await commit transaction

            // return success
            _logger.LogInformation("Parameter updated");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating parameter");
            // await rollback transaction

            throw new BadRequestException("Error updating parameter");
        }
    }
}

