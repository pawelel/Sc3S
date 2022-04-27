using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface IStuffService
{
    Task<ServiceResponse> ChangeModelOfAsset(int assetId, int modelId);

    Task<ServiceResponse<int>> CreateAsset(AssetUpdateCommand assetUpdateDto);

    Task<ServiceResponse<(int, int)>> CreateAssetCategory(int assetId, int categoryId);

    Task<ServiceResponse<(int, int)>> CreateAssetDetail(AssetDetailQuery assetDetailDto);

    Task<ServiceResponse<int>> CreateCategory(CategoryUpdateCommand categoryUpdateDto);

    Task<ServiceResponse<int>> CreateDetail(DetailUpdateCommand detailUpdateDto);

    Task<ServiceResponse<int>> CreateDevice(DeviceUpdateCommand deviceUpdateDto);

    Task<ServiceResponse<int>> CreateModel(ModelUpdateCommand modelUpdateDto);

    Task<ServiceResponse<(int, int)>> CreateModelParameter(ModelParameterQuery modelParameterDto);

    Task<ServiceResponse<int>> CreateParameter(ParameterUpdateCommand parameterUpdateDto);

    Task<ServiceResponse> DeleteAsset(int assetId);

    Task<ServiceResponse> DeleteAssetCategory(int assetId, int categoryId);

    Task<ServiceResponse> DeleteAssetDetail(int assetId, int detailId);

    Task<ServiceResponse> DeleteCategory(int categoryId);

    Task<ServiceResponse> DeleteDetail(int detailId);

    Task<ServiceResponse> DeleteDevice(int deviceId);

    Task<ServiceResponse> DeleteModel(int modelId);

    Task<ServiceResponse> DeleteModelParameter(int modelId, int parameterId);

    Task<ServiceResponse> DeleteParameter(int parameterId);

    Task<ServiceResponse<AssetQuery>> GetAssetById(int assetId);

    Task<ServiceResponse<IEnumerable<AssetDisplayQuery>>> GetAssetDisplays();

    Task<ServiceResponse<IEnumerable<AssetQuery>>> GetAssets();

    Task<ServiceResponse<IEnumerable<CategoryQuery>>> GetCategories();

    Task<ServiceResponse<IEnumerable<CategoryWithAssetsQuery>>> GetCategoriesWithAssets();

    Task<ServiceResponse<IEnumerable<DeviceWithAssetsQuery>>> GetDevicesWithAssets();

    Task<ServiceResponse<DeviceWithAssetsQuery>> GetDeviceWithAssets(int deviceId);

    Task<ServiceResponse<CategoryQuery>> GetCategoryById(int categoryId);

    Task<ServiceResponse<CategoryWithAssetsQuery>> GetCategoryByIdWithAssets(int categoryId);

    Task<ServiceResponse<DetailQuery>> GetDetailById(int detailId);

    Task<ServiceResponse<IEnumerable<DetailQuery>>> GetDetails();

    Task<ServiceResponse<IEnumerable<DetailWithAssetsQuery>>> GetDetailsWithAssets();

    Task<ServiceResponse<DeviceQuery>> GetDeviceById(int deviceId);

    Task<ServiceResponse<IEnumerable<DeviceQuery>>> GetDevicesWithModels();

    Task<ServiceResponse<ModelQuery>> GetModelById(int modelId);

    Task<ServiceResponse<IEnumerable<ModelQuery>>> GetModels();

    Task<ServiceResponse<IEnumerable<ModelQuery>>> GetModelsWithAssets();

    Task<ServiceResponse<ParameterQuery>> GetParameterById(int parameterId);

    Task<ServiceResponse<IEnumerable<ParameterQuery>>> GetParameters();

    Task<ServiceResponse<IEnumerable<ParameterWithModelsQuery>>> GetParametersWithModels();

    Task<ServiceResponse> MarkDeleteAsset(int assetId);

    Task<ServiceResponse> MarkDeleteAssetCategory(int assetId, int categoryId);

    Task<ServiceResponse> MarkDeleteAssetDetail(int assetId, int detailId);

    Task<ServiceResponse> MarkDeleteCategory(int categoryId);

    Task<ServiceResponse> MarkDeleteDetail(int detailId);

    Task<ServiceResponse> MarkDeleteDevice(string userName, int deviceId);

    Task<ServiceResponse> MarkDeleteModel(int modelId);

    Task<ServiceResponse> MarkDeleteModelParameter(int modelId, int parameterId);

    Task<ServiceResponse> MarkDeleteParameter(int parameterId);

    Task<ServiceResponse> UpdateAsset(AssetUpdateCommand assetUpdateDto);

    Task<ServiceResponse> UpdateAssetCategory(int assetId, int categoryId);

    Task<ServiceResponse> UpdateAssetDetail(AssetDetailQuery assetDetailDto);

    Task<ServiceResponse> UpdateCategory(CategoryUpdateCommand categoryUpdateDto);

    Task<ServiceResponse> UpdateDetail(DetailUpdateCommand detailUpdateDto);

    Task<ServiceResponse> UpdateDevice(DeviceUpdateCommand deviceUpdateDto);

    Task<ServiceResponse> UpdateModel(ModelUpdateCommand modelUpdateDto);

    Task<ServiceResponse> UpdateModelParameter(ModelParameterQuery modelParameterDto);

    Task<ServiceResponse> UpdateParameter(ParameterUpdateCommand parameterUpdateDto);

    Task<ServiceResponse<IEnumerable<DeviceQuery>>> GetDevices();
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

    public async Task<ServiceResponse> ChangeModelOfAsset(int assetId, int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono assetu");
        }
        _context.Entry(asset).State = EntityState.Modified;

        if (asset.ModelId == modelId)
        {
            _logger.LogWarning("Asset already assigned to model");
            return new ServiceResponse(false, "Asset już jest przypisany do tego modelu");
        }
        var model = await _context.Models.Include(a => a.Assets).FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Nie znaleziono modelu");
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
            return new ServiceResponse(true, $"Asset o id id {assetId} został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating asset with id {AssetId}", assetId);

            return new ServiceResponse(false, $"Błąd podczas aktualizacji assetu o id {assetId}");
        }
    }

    public async Task<ServiceResponse<int>> CreateAsset(AssetUpdateCommand assetUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var model = await _context.Models.Include(m => m.Assets).AsNoTracking().FirstOrDefaultAsync(m => m.ModelId == assetUpdateDto.ModelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse<int>(false, -1, "Nie znaleziono modelu");
        }
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model marked as deleted");
            return new ServiceResponse<int>(false, -1, "Model został oznaczony jako usunięty");
        }
        // get coordinate
        var coordinate = await _context.Coordinates.AsNoTracking().FirstOrDefaultAsync(c => c.CoordinateId == assetUpdateDto.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse<int>(false, -1, "Nie znaleziono koordynatu");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate marked as deleted");
            return new ServiceResponse<int>(false, -1, "Koordynat został oznaczony jako usunięty");
        }

        // validate asset name
        var duplicate = model.Assets.Any(a => a.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Asset name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa assetu już istnieje");
        }

        var asset = new Asset
        {
            ModelId = assetUpdateDto.ModelId,
            CoordinateId = assetUpdateDto.CoordinateId,
            Name = assetUpdateDto.Name,
            Description = assetUpdateDto.Description,
            Process = assetUpdateDto.Process,
            Status = assetUpdateDto.Status,
            IsDeleted = false,
            Model = new() { ModelId = assetUpdateDto.ModelId },
            Coordinate = new() { CoordinateId = assetUpdateDto.CoordinateId }
        };
        // create asset
        _context.Attach(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            _logger.LogInformation("Asset with id {AssetId} created", asset.AssetId);
            return new ServiceResponse<int>(true, asset.AssetId, $"Asset o id id {asset.AssetId} został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia assetu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await _context.AssetCategories.FirstOrDefaultAsync(a => a.AssetId == assetId && a.CategoryId == categoryId);
        if (assetCategory != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "AssetCategory już istnieje");
        var category = categoryId < 1 ? null : await _context.Categories.FirstOrDefaultAsync(a => a.CategoryId == categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono kategorii");
        }
        var asset = assetId < 1 ? null : await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono assetu");
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

            return new ServiceResponse<(int, int)>(true, (assetId, categoryId), $"AssetCategory został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assetCategory");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia assetCategory");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateAssetDetail(AssetDetailQuery assetDetailDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail != null)
        {
            _logger.LogWarning("AssetDetail already exists");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "AssetDetail już istnieje");
        }
        var asset = await _context.Assets.FindAsync(assetDetailDto.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono assetu");
        }
        var detail = await _context.Details.FindAsync(assetDetailDto.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono szczegółu");
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

            return new ServiceResponse<(int, int)>(true, (assetDetail.AssetId, assetDetail.DetailId), $"AssetDetail został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assetDetail");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas tworzenia assetDetail");
        }
    }

    public async Task<ServiceResponse<int>> CreateCategory(CategoryUpdateCommand categoryUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // validate category name
        var duplicate = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Category name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa kategorii już istnieje");
        }

        var category = new Category
        {
            Name = categoryUpdateDto.Name,
            Description = categoryUpdateDto.Description,
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
            return new ServiceResponse<int>(true, category.CategoryId, $"Kategoria została utworzona");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia kategorii");
        }
    }

    public async Task<ServiceResponse<int>> CreateDetail(DetailUpdateCommand detailUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // validate detail name
        var duplicate = await _context.Details.AnyAsync(d => d.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Detail name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa szczegółu już istnieje");
        }

        var detail = new Detail
        {
            Name = detailUpdateDto.Name,
            Description = detailUpdateDto.Description,
            IsDeleted = false
        };
        // create detail
        _context.Details.Attach(detail);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Detail with id {DetailId} created", detail.DetailId);
            return new ServiceResponse<int>(true, detail.DetailId, $"Szczegół został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating detail");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia szczegółu");
        }
    }

    public async Task<ServiceResponse<int>> CreateDevice(DeviceUpdateCommand deviceUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // validate device name
        var duplicate = await _context.Devices.AnyAsync(d => d.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Device name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa urządzenia już istnieje");
        }

        var device = new Device
        {
            Name = deviceUpdateDto.Name,
            Description = deviceUpdateDto.Description,
            IsDeleted = false,
            CreatedBy = deviceUpdateDto.UserName,
            UpdatedBy = deviceUpdateDto.UserName
        };
        // create device
        _context.Devices.Attach(device);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device with id {DeviceId} created", device.DeviceId);
            return new ServiceResponse<int>(true, device.DeviceId, $"Urządzenie zostało utworzone");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia urządzenia");
        }
    }

    public async Task<ServiceResponse<int>> CreateModel(ModelUpdateCommand modelUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get device
        var device = await _context.Devices.Include(d => d.Models).AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == modelUpdateDto.DeviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse<int>(false, -1, "Nie znaleziono urządzenia");
        }

        // validate model name
        var duplicate = device.Models.Any(m => m.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Model name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa modelu już istnieje");
        }
        // check if device is marked as deleted

        var model = new Model
        {
            DeviceId = modelUpdateDto.DeviceId,
            Device = new() { DeviceId = modelUpdateDto.DeviceId },
            Name = modelUpdateDto.Name,
            Description = modelUpdateDto.Description,
            IsDeleted = false
        };
        // create model
        _context.Attach(model);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Model with id {ModelId} created", model.ModelId);
            return new ServiceResponse<int>(true, model.ModelId, $"Model został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating model");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia modelu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateModelParameter(ModelParameterQuery modelParameterDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        if (modelParameter != null)
        {
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Model już posiada parametr");
        }
        var model = await _context.Models.FindAsync(modelParameterDto.ModelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono modelu");
        }
        var parameter = await _context.Parameters.FindAsync(modelParameterDto.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Nie znaleziono parametru");
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

            return new ServiceResponse<(int, int)>(true, (modelParameter.ModelId, modelParameter.ParameterId), $"Parametr został dodany do modelu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating modelParameter");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas dodawania parametru do modelu");
        }
    }

    public async Task<ServiceResponse<int>> CreateParameter(ParameterUpdateCommand parameterUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // validate parameter name
        var duplicate = await _context.Parameters.AnyAsync(p => p.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Parameter name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa parametru już istnieje");
        }
        var parameter = new Parameter
        {
            Name = parameterUpdateDto.Name,
            Description = parameterUpdateDto.Description,
            IsDeleted = false
        };
        // create parameter
        _context.Parameters.Attach(parameter);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Parameter with id {ParameterId} created", parameter.ParameterId);
            return new ServiceResponse<int>(true, parameter.ParameterId, $"Parametr został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia parametru");
        }
    }

    public async Task<ServiceResponse> DeleteAsset(int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono assetu");
        }
        // check if asset is marked as deleted
        if (asset.IsDeleted == false)
        {
            _logger.LogWarning("Asset is not marked as deleted");
            return new ServiceResponse(false, "Asset nie jest oznaczony jako usunięty");
        }
        // delete asset
        _context.Assets.Remove(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Asset with id {AssetId} deleted", asset.AssetId);
            return new ServiceResponse(true, "Asset został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset");

            return new ServiceResponse(false, "Błąd podczas usuwania assetu");
        }
    }

    public async Task<ServiceResponse> DeleteAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        // get assetCategory
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            return new ServiceResponse(false, "Nie znaleziono assetCategory");
        }
        if (assetCategory.IsDeleted == false)
        {
            _logger.LogWarning("AssetCategory is not marked as deleted");
            return new ServiceResponse(false, "AssetCategory nie jest oznaczony jako usunięty");
        }

        _context.AssetCategories.Remove(assetCategory);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetCategory deleted");
            return new ServiceResponse(true, "AssetCategory został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetCategory");

            return new ServiceResponse(false, "Błąd podczas usuwania assetCategory");
        }
    }

    public async Task<ServiceResponse> DeleteAssetDetail(int assetId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetId, detailId);

        // get assetDetail
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            return new ServiceResponse(false, "Nie znaleziono assetDetail");
        }
        if (assetDetail.IsDeleted == false)
        {
            _logger.LogWarning("AssetDetail is not marked as deleted");
            return new ServiceResponse(false, "AssetDetail nie jest oznaczony jako usunięty");
        }

        try
        {
            _context.AssetDetails.Remove(assetDetail);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetDetail deleted");
            return new ServiceResponse(true, "AssetDetail został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assetDetail");

            return new ServiceResponse(false, "Błąd podczas usuwania assetDetail");
        }
    }

    public async Task<ServiceResponse> DeleteCategory(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get category
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii");
        }
        // check if category is marked as deleted
        if (category.IsDeleted == false)
        {
            _logger.LogWarning("Category is not marked as deleted");
            return new ServiceResponse(false, "Kategoria nie jest oznaczona jako usunięta");
        }
        // delete category
        _context.Categories.Remove(category);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category with id {CategoryId} deleted", category.CategoryId);
            return new ServiceResponse(true, "Kategoria została usunięta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");

            return new ServiceResponse(false, "Błąd podczas usuwania kategorii");
        }
    }

    public async Task<ServiceResponse> DeleteDetail(int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get detail
        var detail = await _context.Details.FindAsync(detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu");
        }
        // check if detail is marked as deleted
        if (detail.IsDeleted == false)
        {
            _logger.LogWarning("Detail is not marked as deleted");
            return new ServiceResponse(false, "Szczegół nie jest oznaczony jako usunięty");
        }
        // delete detail
        _context.Details.Remove(detail);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Detail with id {DetailId} deleted", detail.DetailId);
            return new ServiceResponse(true, "Szczegół został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting detail");

            return new ServiceResponse(false, "Błąd podczas usuwania szczegółu");
        }
    }

    public async Task<ServiceResponse> DeleteDevice(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get device
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse(false, "Nie znaleziono urządzenia");
        }
        // check if device is marked as deleted
        if (device.IsDeleted == false)
        {
            _logger.LogWarning("Device is not marked as deleted");
            return new ServiceResponse(false, "Urządzenie nie jest oznaczone jako usunięte");
        }
        // delete device
        _context.Devices.Remove(device);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device with id {DeviceId} deleted", device.DeviceId);
            return new ServiceResponse(true, "Urządzenie zostało usunięte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device");

            return new ServiceResponse(false, "Błąd podczas usuwania urządzenia");
        }
    }

    public async Task<ServiceResponse> DeleteModel(int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var model = await _context.Models.FindAsync(modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Nie znaleziono modelu");
        }
        // check if model is marked as deleted
        if (model.IsDeleted == false)
        {
            _logger.LogWarning("Model is not marked as deleted");
            return new ServiceResponse(false, "Model nie jest oznaczony jako usunięty");
        }
        // delete model
        _context.Models.Remove(model);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Model with id {ModelId} deleted", model.ModelId);
            return new ServiceResponse(true, "Model został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model");

            return new ServiceResponse(false, "Błąd podczas usuwania modelu");
        }
    }

    public async Task<ServiceResponse> DeleteModelParameter(int modelId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelId, parameterId);

        // get modelParameter
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            return new ServiceResponse(false, "Nie znaleziono modelu parametru");
        }
        if (modelParameter.IsDeleted)
        {
            try
            {
                _context.ModelParameters.Remove(modelParameter);
                // save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("ModelParameter deleted");
                return new ServiceResponse(true, "Parametr modelu został usunięty");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting modelParameter");

                return new ServiceResponse(false, "Błąd podczas usuwania parametru modelu");
            }
        }
        _logger.LogWarning("ModelParameter is not marked as deleted");
        return new ServiceResponse(false, "Parametr modelu nie jest oznaczony jako usunięty");
    }

    public async Task<ServiceResponse> DeleteParameter(int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameter
        var parameter = await _context.Parameters.FindAsync(parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse(false, "Nie znaleziono parametru");
        }
        // check if parameter is marked as deleted
        if (parameter.IsDeleted == false)
        {
            _logger.LogWarning("Parameter is not marked as deleted");
            return new ServiceResponse(false, "Parametr nie jest oznaczony jako usunięty");
        }
        // delete parameter
        _context.Parameters.Remove(parameter);

        try
        {
            // save changes
            await _context.SaveChangesAsync();
            _logger.LogInformation("Parameter with id {ParameterId} deleted", parameter.ParameterId);
            return new ServiceResponse(true, "Parametr został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter");

            return new ServiceResponse(false, "Błąd podczas usuwania parametru");
        }
    }

    public async Task<ServiceResponse<AssetQuery>> GetAssetById(int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset
        var asset = await _context.Assets.AsNoTracking().Select(a => new AssetQuery
        {
            AssetId = a.AssetId,
            Name = a.Name,
            Description = a.Description,
            IsDeleted = a.IsDeleted,
            UpdatedBy = a.UpdatedBy,
            Status = a.Status,
            CoordinateId = a.CoordinateId,
            ModelId = a.ModelId,
            Process = a.Process
        }).FirstOrDefaultAsync(a => a.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<AssetQuery>(false, null, "Nie znaleziono zasobu");
        }
        // return asset
        _logger.LogInformation("Asset with id {AssetId} found", asset.AssetId);
        return new ServiceResponse<AssetQuery>(true, asset, "Zasób został znaleziony");
    }

    public async Task<ServiceResponse<IEnumerable<AssetDisplayQuery>>> GetAssetDisplays()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset
        var query = await _context.Assets
            .AsNoTracking()
            .Select(a => new AssetDisplayQuery
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
                UpdatedBy = a.UpdatedBy,
                Categories = a.AssetCategories.Select(ac => new AssetCategoryDisplayQuery
                {
                    Name = ac.Category.Name,
                    AssetId = ac.AssetId,
                    UpdatedBy = ac.UpdatedBy,
                    CategoryId = ac.CategoryId,
                    Description = ac.Category.Description,
                    IsDeleted = ac.IsDeleted
                }).ToList(),
                Details = a.AssetDetails.Select(ad => new AssetDetailDisplayQuery
                {
                    Name = ad.Detail.Name,
                    Value = ad.Value,
                    DetailId = ad.DetailId,
                    UpdatedBy = ad.UpdatedBy,
                    AssetId = ad.AssetId,
                    Description = ad.Detail.Description,
                    IsDeleted = ad.IsDeleted
                }).ToList(),
                Parameters = a.Model.ModelParameters.Select(mp => new ModelParameterDisplayQuery
                {
                    Name = mp.Parameter.Name,
                    Value = mp.Value,
                    ParameterId = mp.ParameterId,
                    UpdatedBy = mp.UpdatedBy,
                    ModelId = mp.ModelId,
                    Description = mp.Parameter.Description,
                    IsDeleted = mp.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No assets found");
            return new ServiceResponse<IEnumerable<AssetDisplayQuery>>(false, null, "Nie znaleziono zasobów");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return new ServiceResponse<IEnumerable<AssetDisplayQuery>>(true, query, "Zasoby zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<AssetQuery>>> GetAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assets
        var assets = await _context.Assets
            .AsNoTracking()
            .Select(a => new AssetQuery
            {
                AssetId = a.AssetId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UpdatedBy = a.UpdatedBy,
                Status = a.Status,
                CoordinateId = a.CoordinateId,
                ModelId = a.ModelId,
                Process = a.Process
            }).ToListAsync();
        if (assets is null)
        {
            _logger.LogWarning("No assets found");
            return new ServiceResponse<IEnumerable<AssetQuery>>(false, null, "Nie znaleziono zasobów");
        }
        // return assets
        _logger.LogInformation("Assets found");
        return new ServiceResponse<IEnumerable<AssetQuery>>(true, assets, "Zasoby zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<CategoryQuery>>> GetCategories()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get categories
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryQuery
            {
                Name = c.Name,
                Description = c.Description,
                CategoryId = c.CategoryId,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No categories found");
            return new ServiceResponse<IEnumerable<CategoryQuery>>(false, null, "Nie znaleziono kategorii");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return new ServiceResponse<IEnumerable<CategoryQuery>>(true, query, "Kategorie zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<CategoryWithAssetsQuery>>> GetCategoriesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get categories
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryWithAssetsQuery
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Assets = c.AssetCategories.Select(ac => new AssetQuery
                {
                    Name = ac.Asset.Name,
                    Description = ac.Asset.Description,
                    AssetId = ac.AssetId,
                    Status = ac.Asset.Status,
                    UpdatedBy = ac.UpdatedBy,
                    Process = ac.Asset.Process,
                    IsDeleted = ac.Asset.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No categories found");
            return new ServiceResponse<IEnumerable<CategoryWithAssetsQuery>>(false, null, "Nie znaleziono kategorii");
        }
        // return categories
        _logger.LogInformation("Categories found");
        return new ServiceResponse<IEnumerable<CategoryWithAssetsQuery>>(true, query, "Kategorie zostały znalezione");
    }

    public async Task<ServiceResponse<CategoryQuery>> GetCategoryById(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get category
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryQuery
            {
                Name = c.Name,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Description = c.Description,
                CategoryId = c.CategoryId
            }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        if (query == null)
        {
            _logger.LogWarning("No category found");
            return new ServiceResponse<CategoryQuery>(false, null, "Nie znaleziono kategorii");
        }
        // return category
        _logger.LogInformation("Category found");
        return new ServiceResponse<CategoryQuery>(true, query, "Kategoria została znaleziona");
    }

    public async Task<ServiceResponse<CategoryWithAssetsQuery>> GetCategoryByIdWithAssets(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get category
        var query = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryWithAssetsQuery
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                UpdatedBy = c.UpdatedBy,
                IsDeleted = c.IsDeleted,
                Assets = c.AssetCategories.Select(ac => new AssetQuery
                {
                    Name = ac.Asset.Name,
                    Description = ac.Asset.Description,
                    AssetId = ac.AssetId,
                    UpdatedBy = ac.UpdatedBy,
                    IsDeleted = ac.IsDeleted,
                    Status = ac.Asset.Status,
                    Process = ac.Asset.Process
                }).ToList()
            }).FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        if (query == null)
        {
            _logger.LogWarning("No category found");
            return new ServiceResponse<CategoryWithAssetsQuery>(false, null, "Nie znaleziono kategorii");
        }
        // return category
        _logger.LogInformation("Category found");
        return new ServiceResponse<CategoryWithAssetsQuery>(true, query, "Kategoria została znaleziona");
    }

    public async Task<ServiceResponse<DetailQuery>> GetDetailById(int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get detail
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailQuery
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UpdatedBy = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).FirstOrDefaultAsync(d => d.DetailId == detailId);
        if (query == null)
        {
            _logger.LogWarning("No detail found");
            return new ServiceResponse<DetailQuery>(false, null, "Nie znaleziono szczegółu");
        }
        // return detail
        _logger.LogInformation("Detail found");
        return new ServiceResponse<DetailQuery>(true, query, "Szczegół został znaleziony");
    }

    public async Task<ServiceResponse<IEnumerable<DetailQuery>>> GetDetails()
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get details
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailQuery
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UpdatedBy = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No details found");
            return new ServiceResponse<IEnumerable<DetailQuery>>(false, null, "Nie znaleziono szczegółów");
        }

        // return details
        _logger.LogInformation("Details found");
        return new ServiceResponse<IEnumerable<DetailQuery>>(true, query, "Szczegóły zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<DetailWithAssetsQuery>>> GetDetailsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get details
        var query = await _context.Details
            .AsNoTracking()
            .Select(d => new DetailWithAssetsQuery
            {
                DetailId = d.DetailId,
                Name = d.Name,
                UpdatedBy = d.UpdatedBy,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                Assets = d.AssetDetails.Select(ad => new AssetQuery
                {
                    Name = ad.Asset.Name,
                    Description = ad.Asset.Description,
                    AssetId = ad.AssetId,
                    UpdatedBy = ad.UpdatedBy,
                    Status = ad.Asset.Status,
                    Process = ad.Asset.Process,
                    IsDeleted = ad.Asset.IsDeleted
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No details found");
            return new ServiceResponse<IEnumerable<DetailWithAssetsQuery>>(false, null, "Nie znaleziono szczegółów");
        }
        // return details
        _logger.LogInformation("Details found");
        return new ServiceResponse<IEnumerable<DetailWithAssetsQuery>>(true, query, "Szczegóły zostały znalezione");
    }

    public async Task<ServiceResponse<DeviceQuery>> GetDeviceById(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get device
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceQuery
            {
                DeviceId = d.DeviceId,
                UpdatedBy = d.UpdatedBy,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted
            }).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (query == null)
        {
            _logger.LogWarning("No device found");
            return new ServiceResponse<DeviceQuery>(false, null, "Nie znaleziono urządzenia");
        }
        // return device
        _logger.LogInformation("Device found");
        return new ServiceResponse<DeviceQuery>(true, query, "Urządzenie zostało znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<DeviceQuery>>> GetDevices()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceQuery
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UpdatedBy = d.UpdatedBy,
                CreatedOn = d.CreatedOn,
                UpdatedOn = d.UpdatedOn
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            return new ServiceResponse<IEnumerable<DeviceQuery>>(false, null, "Nie znaleziono urządzeń");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return new ServiceResponse<IEnumerable<DeviceQuery>>(true, query, "Urządzenia zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<DeviceQuery>>> GetDevicesWithModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceQuery
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UpdatedBy = d.UpdatedBy,
                Models = d.Models.Select(m => new ModelQuery
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    IsDeleted = m.IsDeleted,
                    UpdatedBy = m.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            return new ServiceResponse<IEnumerable<DeviceQuery>>(false, null, "Nie znaleziono urządzeń");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return new ServiceResponse<IEnumerable<DeviceQuery>>(true, query, "Urządzenia zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<DeviceWithAssetsQuery>>> GetDevicesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get devices
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceWithAssetsQuery
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UpdatedBy = d.UpdatedBy,
                Assets = d.Models.SelectMany(m => m.Assets).Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No devices found");
            return new ServiceResponse<IEnumerable<DeviceWithAssetsQuery>>(false, null, "Nie znaleziono urządzeń");
        }
        // return devices
        _logger.LogInformation("Devices found");
        return new ServiceResponse<IEnumerable<DeviceWithAssetsQuery>>(true, query, "Urządzenia zostały znalezione");
    }

    public async Task<ServiceResponse<DeviceWithAssetsQuery>> GetDeviceWithAssets(int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get device
        var query = await _context.Devices
            .AsNoTracking()
            .Select(d => new DeviceWithAssetsQuery
            {
                DeviceId = d.DeviceId,
                Name = d.Name,
                Description = d.Description,
                IsDeleted = d.IsDeleted,
                UpdatedBy = d.UpdatedBy,
                Assets = d.Models.SelectMany(m => m.Assets).Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy
                }).ToList()
            }).FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (query is null)
        {
            _logger.LogWarning("No device found");
            return new ServiceResponse<DeviceWithAssetsQuery>(false, null, "Nie znaleziono urządzenia");
        }
        // return device
        _logger.LogInformation("Device found");
        return new ServiceResponse<DeviceWithAssetsQuery>(true, query, "Urządzenie zostało znalezione");
    }

    public async Task<ServiceResponse<ModelQuery>> GetModelById(int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelQuery
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UpdatedBy = m.UpdatedBy
            }).FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (query == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse<ModelQuery>(false, null, "Nie znaleziono modelu");
        }
        // return model
        _logger.LogInformation("Model found");
        return new ServiceResponse<ModelQuery>(true, query, "Model został znaleziony");
    }

    public async Task<ServiceResponse<IEnumerable<ModelQuery>>> GetModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get models
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelQuery
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UpdatedBy = m.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No models found");
            return new ServiceResponse<IEnumerable<ModelQuery>>(false, null, "Nie znaleziono modeli");
        }
        // return models
        _logger.LogInformation("Models found");
        return new ServiceResponse<IEnumerable<ModelQuery>>(true, query, "Modele zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<ModelQuery>>> GetModelsWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get models
        var query = await _context.Models
            .AsNoTracking()
            .Select(m => new ModelQuery
            {
                ModelId = m.ModelId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UpdatedBy = m.UpdatedBy,
                Assets = m.Assets.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No models found");
            return new ServiceResponse<IEnumerable<ModelQuery>>(false, null, "Nie znaleziono modeli");
        }
        // return models
        _logger.LogInformation("Models found");
        return new ServiceResponse<IEnumerable<ModelQuery>>(true, query, "Modele zostały znalezione");
    }

    public async Task<ServiceResponse<ParameterQuery>> GetParameterById(int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameter
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(m => new ParameterQuery
            {
                ParameterId = m.ParameterId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UpdatedBy = m.UpdatedBy
            }).FirstOrDefaultAsync(m => m.ParameterId == parameterId);
        if (query == null)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse<ParameterQuery>(false, null, "Nie znaleziono parametru");
        }
        // return parameter
        _logger.LogInformation("Parameter found");
        return new ServiceResponse<ParameterQuery>(true, query, "Parametr został znaleziony");
    }

    public async Task<ServiceResponse<IEnumerable<ParameterQuery>>> GetParameters()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameters
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(m => new ParameterQuery
            {
                ParameterId = m.ParameterId,
                Name = m.Name,
                Description = m.Description,
                IsDeleted = m.IsDeleted,
                UpdatedBy = m.UpdatedBy
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No parameters found");
            return new ServiceResponse<IEnumerable<ParameterQuery>>(false, null, "Nie znaleziono parametrów");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return new ServiceResponse<IEnumerable<ParameterQuery>>(true, query, "Parametry zostały znalezione");
    }

    public async Task<ServiceResponse<IEnumerable<ParameterWithModelsQuery>>> GetParametersWithModels()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameters
        var query = await _context.Parameters
            .AsNoTracking()
            .Select(p => new ParameterWithModelsQuery
            {
                ParameterId = p.ParameterId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UpdatedBy = p.UpdatedBy,
                Models = p.ModelParameters.Select(mp => new ModelQuery
                {
                    ModelId = mp.ModelId,
                    Name = mp.Model.Name,
                    Description = mp.Model.Description,
                    IsDeleted = mp.Model.IsDeleted,
                    UpdatedBy = mp.Model.UpdatedBy
                }).ToList()
            }).ToListAsync();
        if (query is null)
        {
            _logger.LogWarning("No parameters found");
            return new ServiceResponse<IEnumerable<ParameterWithModelsQuery>>(false, null, "Nie znaleziono parametrów");
        }
        // return parameters
        _logger.LogInformation("Parameters found");
        return new ServiceResponse<IEnumerable<ParameterWithModelsQuery>>(true, query, "Parametry zostały znalezione");
    }

    public async Task<ServiceResponse> MarkDeleteAsset(int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset
        var asset = await _context.Assets
            .FirstOrDefaultAsync(m => m.AssetId == assetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono zasobu");
        }
        // check if asset is already deleted
        if (asset.IsDeleted)
        {
            _logger.LogWarning("Asset is already marked as deleted");
            return new ServiceResponse(false, "Zasób jest już oznaczony jako usunięty");
        }
        // mark asset as deleted
        asset.IsDeleted = true;
        // update asset
        _context.Update(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Asset marked as deleted");
            return new ServiceResponse(true, "Zasób został oznaczony jako usunięty");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking asset as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania zasobu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii zasobu");
        }
        // check if assetCategory is already deleted
        if (assetCategory.IsDeleted)
        {
            _logger.LogWarning("AssetCategory already marked as deleted");
            return new ServiceResponse(false, "Kategoria zasobu jest już oznaczona jako usunięta");
        }
        assetCategory.IsDeleted = true;
        _context.Update(assetCategory);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetCategory marked as deleted");
            return new ServiceResponse(true, "Kategoria zasobu została oznaczona jako usunięta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetCategory as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania kategorii zasobu jako usuniętej");
        }
    }

    public async Task<ServiceResponse> MarkDeleteAssetDetail(int assetId, int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetDetail
        var assetDetail = await _context.AssetDetails.FindAsync(assetId, detailId);
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu zasobu");
        }
        // check if assetDetail is already deleted
        if (assetDetail.IsDeleted)
        {
            _logger.LogWarning("AssetDetail already deleted");
            return new ServiceResponse(false, "Szczegół zasobu jest już oznaczony jako usunięty");
        }
        assetDetail.IsDeleted = true;
        _context.Update(assetDetail);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("AssetDetail marked as deleted");
            return new ServiceResponse(true, "Szczegół zasobu został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetDetail as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania szczegółu zasobu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCategory(int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get category
        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.CategoryId == categoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii");
        }
        // check if category is already deleted
        if (category.IsDeleted)
        {
            _logger.LogWarning("Category is already deleted");
            return new ServiceResponse(false, "Kategoria jest już oznaczona jako usunięta");
        }
        // mark category as deleted
        category.IsDeleted = true;
        // update category
        _context.Update(category);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Category marked as deleted");
            return new ServiceResponse(true, "Kategoria została oznaczona jako usunięta");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking category as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania kategorii jako usuniętej");
        }
    }

    public async Task<ServiceResponse> MarkDeleteDetail(int detailId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get detail
        var detail = await _context.Details
            .FirstOrDefaultAsync(m => m.DetailId == detailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu");
        }
        // check if detail is already deleted
        if (detail.IsDeleted)
        {
            _logger.LogWarning("Detail is already deleted");
            return new ServiceResponse(false, "Szczegół jest już oznaczony jako usunięty");
        }
        // mark detail as deleted
        detail.IsDeleted = true;
        // update detail
        _context.Update(detail);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Detail marked as deleted");
            return new ServiceResponse(true, "Szczegół został oznaczony jako usunięty");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking detail as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania szczegółu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteDevice(string userId, int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get device
        var device = await _context.Devices
            .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse(false, "Nie znaleziono urządzenia");
        }
        // check if device is already deleted
        if (device.IsDeleted)
        {
            _logger.LogWarning("Device is already deleted");
            return new ServiceResponse(false, "Urządzenie jest już oznaczone jako usunięte");
        }
        // mark device as deleted
        device.IsDeleted = true;
        device.UpdatedBy = userId;
        // update device
        _context.Update(device);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Device marked as deleted");
            return new ServiceResponse(true, "Urządzenie zostało oznaczone jako usunięte");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking device as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania urządzenia jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteModel(int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var model = await _context.Models
            .FirstOrDefaultAsync(m => m.ModelId == modelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Nie znaleziono modelu");
        }
        // check if model is already deleted
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model is already deleted");
            return new ServiceResponse(false, "Model jest już oznaczony jako usunięty");
        }
        // mark model as deleted
        model.IsDeleted = true;
        // update model
        _context.Update(model);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Model marked as deleted");
            return new ServiceResponse(true, "Model został oznaczony jako usunięty");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking model as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania modelu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteModelParameter(int modelId, int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelId, parameterId);
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            return new ServiceResponse(false, "Nie znaleziono modelu parametru");
        }
        if (modelParameter.IsDeleted)
        {
            _logger.LogWarning("ModelParameter already deleted");
            return new ServiceResponse(false, "Model parametru jest już oznaczony jako usunięty");
        }
        modelParameter.IsDeleted = true;
        _context.Update(modelParameter);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("ModelParameter marked as deleted");
            return new ServiceResponse(true, "Model parametru został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking modelParameter as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania modelu parametru jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteParameter(int parameterId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameter
        var parameter = await _context.Parameters
            .FirstOrDefaultAsync(m => m.ParameterId == parameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse(false, "Nie znaleziono parametru");
        }
        // check if parameter is already deleted
        if (parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter is already deleted");
            return new ServiceResponse(false, "Parametr jest już oznaczony jako usunięty");
        }
        // mark parameter as deleted
        parameter.IsDeleted = true;
        // update parameter
        _context.Update(parameter);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Parameter marked as deleted");
            return new ServiceResponse(true, "Parametr został oznaczony jako usunięty");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking parameter as deleted");

            return new ServiceResponse(false, "Błąd podczas oznaczania parametru jako usuniętego");
        }
    }

    public async Task<ServiceResponse> UpdateAsset(AssetUpdateCommand assetUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get asset with all related data
        var asset = await _context.Assets.FirstOrDefaultAsync(m => m.AssetId == assetUpdateDto.AssetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono zasobu");
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
        var duplicateName = await _context.Assets.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Name.ToLower().Trim() == assetUpdateDto.Name.ToLower().Trim() && m.AssetId != assetUpdateDto.AssetId);
        if (duplicateName != null)
        {
            _logger.LogWarning("Asset name already exists");
            return new ServiceResponse(false, "Nazwa zasobu jest już zajęta");
        }

        asset.Name = assetUpdateDto.Name;
        asset.Description = assetUpdateDto.Description;
        asset.IsDeleted = false;
        // update asset
        _context.Update(asset);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Asset updated");
            return new ServiceResponse(true, "Zasób został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating asset");
            return new ServiceResponse(false, "Błąd podczas aktualizacji zasobu");
        }
    }

    public async Task<ServiceResponse> UpdateAssetCategory(int assetId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get assetCategory
        var assetCategory = await _context.AssetCategories.FindAsync(assetId, categoryId);
        if (assetCategory == null)
        {
            _logger.LogWarning("AssetCategory not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii zasobu");
        }
        if (!assetCategory.IsDeleted)
            return new ServiceResponse(false, "Kategoria zasobu jest już przypisana do zasobu");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono zasobu");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii");
        }
        assetCategory.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Kategoria zasobu została przypisana do zasobu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetCategory");
            return new ServiceResponse(false, "Błąd podczas aktualizacji kategorii zasobu");
        }
    }

    public async Task<ServiceResponse> UpdateAssetDetail(AssetDetailQuery assetDetailDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var assetDetail = await _context.AssetDetails.FindAsync(assetDetailDto.AssetId, assetDetailDto.DetailId);
        if (assetDetail == null)
        {
            _logger.LogWarning("AssetDetail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu zasobu");
        }
        var asset = await _context.Assets.FindAsync(assetDetailDto.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Nie znaleziono zasobu");
        }
        var detail = await _context.Details.FindAsync(assetDetailDto.DetailId);
        if (detail == null || detail.IsDeleted)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu");
        }

        assetDetail.IsDeleted = false;
        assetDetail.Value = assetDetailDto.Value;

        // save changes

        try
        {
            _context.AssetDetails.Update(assetDetail);
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Szczegół zasobu został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetDetail");
            return new ServiceResponse(false, "Błąd podczas aktualizacji szczegółu zasobu");
        }
    }

    public async Task<ServiceResponse> UpdateCategory(CategoryUpdateCommand categoryUpdateDto)
    {
        // dodatkowy int
        // duplikat nazwy
        await using var _context = await _factory.CreateDbContextAsync();

        // get category with all related data
        var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == categoryUpdateDto.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Nie znaleziono kategorii");
        }
        // check if category name from dto is already taken
        var duplicate = await _context.Categories.FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == categoryUpdateDto.Name.ToLower().Trim() && a.CategoryId != categoryUpdateDto.CategoryId);
        if (duplicate != null)
        {
            _logger.LogWarning("Category name already exists");
            return new ServiceResponse(false, "Nazwa kategorii jest już zajęta");
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

            // return success
            _logger.LogInformation("Category updated");
            return new ServiceResponse(true, "Kategoria została zaktualizowana");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating category");

            return new ServiceResponse(false, "Błąd podczas aktualizacji kategorii");
        }
    }

    public async Task<ServiceResponse> UpdateDetail(DetailUpdateCommand detailUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get detail
        var detail = await _context.Details.AsNoTracking().FirstOrDefaultAsync(m => m.DetailId == detailUpdateDto.DetailId);
        if (detail == null)
        {
            _logger.LogWarning("Detail not found");
            return new ServiceResponse(false, "Nie znaleziono szczegółu");
        }
        // check if detail name from dto is already taken
        var duplicate = await _context.Details.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == detailUpdateDto.Name.ToLower().Trim() && a.DetailId != detailUpdateDto.DetailId);
        if (duplicate != null)
        {
            _logger.LogWarning("Detail name already exists");
            return new ServiceResponse(false, "Nazwa szczegółu jest już zajęta");
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
            // return success
            _logger.LogInformation("Detail updated");
            return new ServiceResponse(true, "Szczegół został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating detail");

            return new ServiceResponse(false, "Błąd podczas aktualizacji szczegółu");
        }
    }

    public async Task<ServiceResponse> UpdateDevice(DeviceUpdateCommand deviceUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get device
        var device = await _context.Devices.AsNoTracking().FirstOrDefaultAsync(m => m.DeviceId == deviceUpdateDto.DeviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse(false, "Nie znaleziono urządzenia");
        }
        // check if device name from dto is already taken
        var duplicateName = await _context.Devices.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == deviceUpdateDto.Name.ToLower().Trim() && a.DeviceId != deviceUpdateDto.DeviceId);
        if (duplicateName != null)
        {
            _logger.LogWarning("Device name already exists");
            return new ServiceResponse(false, "Nazwa urządzenia jest już zajęta");
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

            // return success
            _logger.LogInformation("Device updated");
            return new ServiceResponse(true, "Urządzenie zostało zaktualizowane");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating device");

            return new ServiceResponse(false, "Błąd podczas aktualizacji urządzenia");
        }
    }

    public async Task<ServiceResponse> UpdateModel(ModelUpdateCommand modelUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get model
        var model = await _context.Models.FirstOrDefaultAsync(m => m.ModelId == modelUpdateDto.ModelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Nie znaleziono modelu");
        }
        // check if model name from dto is already taken
        var duplicate = await _context.Models.AsNoTracking().FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == modelUpdateDto.Name.ToLower().Trim() && a.ModelId != modelUpdateDto.ModelId);
        if (duplicate != null)
        {
            _logger.LogWarning("Model name already exists");
            return new ServiceResponse(false, "Nazwa modelu jest już zajęta");
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
            // return success
            _logger.LogInformation("Model updated");
            return new ServiceResponse(true, "Model został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating model");

            return new ServiceResponse(false, "Błąd podczas aktualizacji modelu");
        }
    }

    public async Task<ServiceResponse> UpdateModelParameter(ModelParameterQuery modelParameterDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        var modelParameter = await _context.ModelParameters.FindAsync(modelParameterDto.ModelId, modelParameterDto.ParameterId);
        if (modelParameter == null)
        {
            _logger.LogWarning("ModelParameter not found");
            return new ServiceResponse(false, "Nie znaleziono parametru modelu");
        }
        var model = await _context.Models.FindAsync(modelParameterDto.ModelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Nie znaleziono modelu");
        }
        var parameter = await _context.Parameters.FindAsync(modelParameterDto.ParameterId);
        if (parameter == null || parameter.IsDeleted)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse(false, "Nie znaleziono parametru");
        }

        modelParameter.IsDeleted = false;
        modelParameter.Value = modelParameterDto.Value;
        // save changes

        try
        {
            _context.ModelParameters.Update(modelParameter);
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Parametr modelu został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating modelParameter");
            return new ServiceResponse(false, "Błąd podczas aktualizacji parametru modelu");
        }
    }

    public async Task<ServiceResponse> UpdateParameter(ParameterUpdateCommand parameterUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get parameter
        var parameter = await _context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == parameterUpdateDto.ParameterId);
        if (parameter == null)
        {
            _logger.LogWarning("Parameter not found");
            return new ServiceResponse(false, "Nie znaleziono parametru");
        }
        // check if parameter name from dto is already taken
        var duplicate = await _context.Parameters.FirstOrDefaultAsync(a => a.Name.ToLower().Trim() == parameterUpdateDto.Name.ToLower().Trim() && a.ParameterId != parameterUpdateDto.ParameterId);
        if (duplicate != null)
        {
            _logger.LogWarning("Parameter name already exists");
            return new ServiceResponse(false, "Nazwa parametru jest już zajęta");
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

            // return success
            _logger.LogInformation("Parameter updated");
            return new ServiceResponse(true, "Parametr został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating parameter");

            return new ServiceResponse(false, "Błąd podczas aktualizacji parametru");
        }
    }
}