using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface ICommunicateService
{
    Task<ServiceResponse<int>> CreateCommunicate(CommunicateUpdateCommand communicateUpdateDto);

    Task<ServiceResponse<(int, int)>> CreateCommunicateArea(CommunicateAreaUpdateCommand communicateArea);

    Task<ServiceResponse<(int, int)>> CreateCommunicateAsset(CommunicateAssetUpdateCommand communicateAsset);

    Task<ServiceResponse<(int, int)>> CreateCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategory);

    Task<ServiceResponse<(int, int)>> CreateCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinate);

    Task<ServiceResponse<(int, int)>> CreateCommunicateDevice(CommunicateDeviceUpdateCommand communicateDevice);

    Task<ServiceResponse<(int, int)>> CreateCommunicateModel(CommunicateModelUpdateCommand communicateModel);

    Task<ServiceResponse<(int, int)>> CreateCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpace);

    Task<ServiceResponse> DeleteCommunicate(int communicateId);

    Task<ServiceResponse> DeleteCommunicateArea(int communicateId, int areaId);

    Task<ServiceResponse> DeleteCommunicateAsset(int communicateId, int assetId);

    Task<ServiceResponse> DeleteCommunicateCategory(int communicateId, int categoryId);

    Task<ServiceResponse> DeleteCommunicateCoordinate(int communicateId, int coordinateId);

    Task<ServiceResponse> DeleteCommunicateDevice(int communicateId, int deviceId);

    Task<ServiceResponse> DeleteCommunicateModel(int communicateId, int modelId);

    Task<ServiceResponse> DeleteCommunicateSpace(int communicateId, int spaceId);

    Task<ServiceResponse<CommunicateQuery>> GetCommunicateById(int communicateId);

    Task<ServiceResponse<IEnumerable<CommunicateQuery>>> GetCommunicates();

    Task<ServiceResponse<IEnumerable<CommunicateWithAssetsQuery>>> GetCommunicatesWithAssets();

    Task<ServiceResponse> MarkDeleteCommunicate(CommunicateUpdateCommand communicateUpdateDto);

    Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaUpdateCommand communicateArea);

    Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetUpdateCommand communicateAsset);

    Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategory);

    Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinate);

    Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceUpdateCommand communicateDevice);

    Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelUpdateCommand communicateModel);

    Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpace);

    Task<ServiceResponse> UpdateCommunicate(CommunicateUpdateCommand communicateUpdateDto);

    Task<ServiceResponse> UpdateCommunicateArea(CommunicateAreaUpdateCommand communicateArea);

    Task<ServiceResponse> UpdateCommunicateAsset(CommunicateAssetUpdateCommand communicateAsset);

    Task<ServiceResponse> UpdateCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategory);

    Task<ServiceResponse> UpdateCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinate);

    Task<ServiceResponse> UpdateCommunicateDevice(CommunicateDeviceUpdateCommand communicateDevice);

    Task<ServiceResponse> UpdateCommunicateModel(CommunicateModelUpdateCommand communicateModel);

    Task<ServiceResponse> UpdateCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpace);
}

public class CommunicateService : ICommunicateService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<CommunicateService> _logger;

    public CommunicateService(IDbContextFactory<Sc3SContext> factory, ILogger<CommunicateService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResponse<int>> CreateCommunicate(CommunicateUpdateCommand communicateUpdateDto)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // validate category name
        var duplicate = await context.Communicates.AnyAsync(c => c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Communicate name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa komunikatu jest zajęta");
        }

        var communicate = new Communicate
        {
            Name = communicateUpdateDto.Name,
            Description = communicateUpdateDto.Description,
            IsDeleted = false,
            CreatedBy = communicateUpdateDto.UpdatedBy,
            UpdatedBy = communicateUpdateDto.UpdatedBy
        };
        // create category
        context.Communicates.Add(communicate);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate with id {CommunicateId} created", communicate.CommunicateId);

            return new ServiceResponse<int>(true, communicate.CommunicateId, "Komunikat stworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia komunikatu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateArea(CommunicateAreaUpdateCommand communicateAreaUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicateArea
        var communicateArea = await context.CommunicateAreas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateAreaUpdate.CommunicateId && c.AreaId == communicateAreaUpdate.AreaId);
        if (communicateArea != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego obszaru");
        var area = await context.Areas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.AreaId == communicateAreaUpdate.AreaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Obszar nie został znaleziony");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateAreaUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateArea = new CommunicateArea
        {
            CreatedBy = communicateAreaUpdate.UpdatedBy,
            UpdatedBy = communicateAreaUpdate.UpdatedBy,
            CommunicateId = communicateAreaUpdate.CommunicateId,
            AreaId = communicateAreaUpdate.AreaId,
            IsDeleted = false
        };
        context.Add(communicateArea);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateAreaUpdate.CommunicateId, communicateAreaUpdate.AreaId), "Komunikat został przypisany do obszaru");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateArea");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateAsset(CommunicateAssetUpdateCommand communicateAssetUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicateAsset
        var communicateAsset = await context.CommunicateAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateAssetUpdate.CommunicateId && c.AssetId == communicateAssetUpdate.AssetId);
        if (communicateAsset != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego zasobu");
        var asset = await context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.AssetId == communicateAssetUpdate.AssetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zasób nie został znaleziony");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateAssetUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateAsset = new CommunicateAsset
        {
            CreatedBy = communicateAssetUpdate.UpdatedBy,
            UpdatedBy = communicateAssetUpdate.UpdatedBy,
            CommunicateId = communicateAssetUpdate.CommunicateId,
            AssetId = communicateAssetUpdate.AssetId,
            IsDeleted = false
        };
        context.Add(communicateAsset);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateAssetUpdate.CommunicateId, communicateAssetUpdate.AssetId), "Komunikat został przypisany do zasobu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateAsset");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategoryUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicateCategory
        var communicateCategory = await context.CommunicateCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateCategoryUpdate.CommunicateId && c.CategoryId == communicateCategoryUpdate.CategoryId);
        if (communicateCategory != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tej kategorii");
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == communicateCategoryUpdate.CategoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Kategoria nie została znaleziona");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateCategoryUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateCategory = new CommunicateCategory
        {
            CreatedBy = communicateCategoryUpdate.UpdatedBy,
            UpdatedBy = communicateCategoryUpdate.UpdatedBy,
            CommunicateId = communicateCategoryUpdate.CommunicateId,
            CategoryId = communicateCategoryUpdate.CategoryId,
            IsDeleted = false
        };
        context.Add(communicateCategory);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateCategoryUpdate.CommunicateId, communicateCategoryUpdate.CategoryId), "Komunikat został przypisany do kategorii");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateCategory");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinateUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicateCoordinate
        var communicateCoordinate = await context.CommunicateCoordinates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateCoordinateUpdate.CommunicateId && c.CoordinateId == communicateCoordinateUpdate.CoordinateId);
        if (communicateCoordinate != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tej koordynaty");
        var coordinate = await context.Coordinates.AsNoTracking().FirstOrDefaultAsync(c => c.CoordinateId == communicateCoordinateUpdate.CoordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Koordynat nie został znaleziony");
        }
        var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateCoordinateUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateCoordinate = new CommunicateCoordinate
        {
            CreatedBy = communicateCoordinateUpdate.UpdatedBy,
            UpdatedBy = communicateCoordinateUpdate.UpdatedBy,
            CommunicateId = communicateCoordinateUpdate.CommunicateId,
            CoordinateId = communicateCoordinateUpdate.CoordinateId,
            IsDeleted = false
        };
        context.Add(communicateCoordinate);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateCoordinateUpdate.CommunicateId, communicateCoordinateUpdate.CoordinateId), "Komunikat został przypisany do koordynaty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateCoordinate");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateDevice(CommunicateDeviceUpdateCommand communicateDeviceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateDevice = await context.CommunicateDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateDeviceUpdate.CommunicateId && c.DeviceId == communicateDeviceUpdate.DeviceId);
        if (communicateDevice != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego urządzenia");
        var device = await context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.DeviceId == communicateDeviceUpdate.DeviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Urządzenie nie zostało znalezione");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateDeviceUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateDevice = new CommunicateDevice
        {
            CreatedBy = communicateDeviceUpdate.UpdatedBy,
            UpdatedBy = communicateDeviceUpdate.UpdatedBy,
            CommunicateId = communicateDeviceUpdate.CommunicateId,
            DeviceId = communicateDeviceUpdate.DeviceId,
            IsDeleted = false
        };
        context.Add(communicateDevice);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateDeviceUpdate.CommunicateId, communicateDeviceUpdate.DeviceId), "Komunikat został przypisany do urządzenia");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateDevice");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do urządzenia");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateModel(CommunicateModelUpdateCommand communicateModelUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateModel = await context.CommunicateModels
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateModelUpdate.CommunicateId && c.ModelId == communicateModelUpdate.ModelId);
        if (communicateModel != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego modelu");
        var model = await context.Models
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ModelId == communicateModelUpdate.ModelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Model nie został znaleziony");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateModelUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateModel = new CommunicateModel
        {
            CreatedBy = communicateModelUpdate.UpdatedBy,
            UpdatedBy = communicateModelUpdate.UpdatedBy,
            CommunicateId = communicateModelUpdate.CommunicateId,
            ModelId = communicateModelUpdate.ModelId,
            IsDeleted = false
        };
        context.Add(communicateModel);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateModelUpdate.CommunicateId, communicateModelUpdate.ModelId), "Komunikat został przypisany do modelu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicateModel");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do modelu");
        }
    }

    public async Task<ServiceResponse<(int, int)>> CreateCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpaceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateSpace = await context.CommunicateSpaces
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateSpaceUpdate.CommunicateId && c.SpaceId == communicateSpaceUpdate.SpaceId);
        if (communicateSpace != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tej strefy");
        var space = await context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.SpaceId == communicateSpaceUpdate.SpaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Strefa nie została znaleziona");
        }
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateSpaceUpdate.CommunicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateSpace = new CommunicateSpace
        {
            CreatedBy = communicateSpaceUpdate.UpdatedBy,
            UpdatedBy = communicateSpaceUpdate.UpdatedBy,
            CommunicateId = communicateSpaceUpdate.CommunicateId,
            SpaceId = communicateSpaceUpdate.SpaceId,
            IsDeleted = false
        };
        context.Add(communicateSpace);
        // save changes

        try
        {
            await context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateSpaceUpdate.CommunicateId, communicateSpaceUpdate.SpaceId), "Komunikat został przypisany do strefy");
        }
        catch
        {
            _logger.LogError("Error creating communicateSpace");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do strefy");
        }
    }


    public async Task<ServiceResponse> DeleteCommunicate(int communicateId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate
        var communicate = await context.Communicates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if communicate is marked as deleted
        if (communicate.IsDeleted == false)
        {
            _logger.LogWarning("Communicate is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate
        context.Communicates.Remove(communicate);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate with id {CommunicateId} deleted", communicate.CommunicateId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateArea(int communicateId, int areaId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        // get communicate area
        var communicateArea = await context.CommunicateAreas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AreaId == areaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("Communicate area not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateArea is not marked as deleted
        if (communicateArea.IsDeleted == false)
        {
            _logger.LogWarning("Communicate area is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate area
        context.CommunicateAreas.Remove(communicateArea);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate area with id {CommunicateId}, {AreaId}  deleted", communicateId, areaId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate area");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateAsset(int communicateId, int assetId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate asset
        var communicateAsset = await context.CommunicateAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AssetId == assetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("Communicate asset not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateAsset is not marked as deleted
        if (communicateAsset.IsDeleted == false)
        {
            _logger.LogWarning("Communicate asset is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate asset
        context.CommunicateAssets.Remove(communicateAsset);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate asset with id {CommunicateId}, {AssetId}  deleted", communicateId, assetId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate asset");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateCategory(int communicateId, int categoryId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate category
        var communicateCategory = await context.CommunicateCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CategoryId == categoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("Communicate category not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateCategory is not marked as deleted
        if (communicateCategory.IsDeleted == false)
        {
            _logger.LogWarning("Communicate category is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate category
        context.CommunicateCategories.Remove(communicateCategory);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate category with id {CommunicateId}, {CategoryId}  deleted", communicateId, categoryId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate category");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate coordinate
        var communicateCoordinate = await context.CommunicateCoordinates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CoordinateId == coordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("Communicate coordinate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateCoordinate is not marked as deleted
        if (communicateCoordinate.IsDeleted == false)
        {
            _logger.LogWarning("Communicate coordinate is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate coordinate
        context.CommunicateCoordinates.Remove(communicateCoordinate);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate coordinate with id {CommunicateId}, {CoordinateId}  deleted", communicateId, coordinateId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate coordinate");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateDevice(int communicateId, int deviceId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate device
        var communicateDevice = await context.CommunicateDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.DeviceId == deviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("Communicate device not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateDevice is not marked as deleted
        if (communicateDevice.IsDeleted == false)
        {
            _logger.LogWarning("Communicate device is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate device
        context.CommunicateDevices.Remove(communicateDevice);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate device with id {CommunicateId}, {DeviceId}  deleted", communicateId, deviceId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate device");
            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateModel(int communicateId, int modelId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate model
        var communicateModel = await context.CommunicateModels
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.ModelId == modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("Communicate model not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateModel is not marked as deleted
        if (communicateModel.IsDeleted == false)
        {
            _logger.LogWarning("Communicate model is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate model
        context.CommunicateModels.Remove(communicateModel);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate model with id {CommunicateId}, {ModelId}  deleted", communicateId, modelId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate model");

            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse> DeleteCommunicateSpace(int communicateId, int spaceId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate space
        var communicateSpace = await context.CommunicateSpaces
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.SpaceId == spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("Communicate space not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if CommunicateSpace is not marked as deleted
        if (communicateSpace.IsDeleted == false)
        {
            _logger.LogWarning("Communicate space is not marked as deleted");
            return new ServiceResponse(false, "Komunikat nie jest oznaczony jako usunięty");
        }
        // delete communicate space
        context.CommunicateSpaces.Remove(communicateSpace);

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate space with id {CommunicateId}, {SpaceId}  deleted", communicateId, spaceId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate space");
            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse<CommunicateQuery>> GetCommunicateById(int communicateId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicate
        var communicate = await context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateQuery
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                UpdatedOn = c.UpdatedOn
            })
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<CommunicateQuery>(false, null, "Komunikat nie został znaleziony");
        }
        // return communicate
        _logger.LogInformation("Communicate with id {CommunicateId} returned", communicateId);
        return new ServiceResponse<CommunicateQuery>(true, communicate, "Komunikat został zwrócony");
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateQuery>>> GetCommunicates()
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicates
        var communicates = await context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateQuery
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                UpdatedOn = c.UpdatedOn,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            return new ServiceResponse<IEnumerable<CommunicateQuery>>(false, null, "Komunikaty nie zostały znalezione");
        }

        // return communicates
        _logger.LogInformation("Communicates returned");
        return new ServiceResponse<IEnumerable<CommunicateQuery>>(true, communicates, "Komunikaty zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<CommunicateWithAssetsQuery>>> GetCommunicatesWithAssets()
    {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicates
        var communicates = await context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateWithAssetsQuery
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                UpdatedOn = c.UpdatedOn,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn,
                Assets = c.CommunicateAssets.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UpdatedBy = a.Asset.UpdatedBy,
                    UpdatedOn = a.Asset.UpdatedOn,
                    CreatedBy = a.Asset.CreatedBy,
                    CreatedOn = a.Asset.CreatedOn
                }).ToList()
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            return new ServiceResponse<IEnumerable<CommunicateWithAssetsQuery>>(false, null, "Komunikaty nie zostały znalezione");
        }
        // return communicates
        _logger.LogInformation("Communicates returned");
        return new ServiceResponse<IEnumerable<CommunicateWithAssetsQuery>>(true, communicates, "Komunikaty zostały zwrócone");
    }

    public async Task<ServiceResponse> MarkDeleteCommunicate(CommunicateUpdateCommand communicateUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        // check if Communicate is marked as deleted
        if (communicate.IsDeleted == true)
        {
            _logger.LogWarning("Communicate is marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        // mark Communicate as deleted
        communicate.IsDeleted = true;
        communicate.UpdatedBy = communicateUpdate.UpdatedBy;

        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", communicateUpdate.CommunicateId);
            return new ServiceResponse(true, "Komunikat został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate as deleted");
            return new ServiceResponse(false, "Błąd podczas oznaczania komunikatu jako usunięty");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaUpdateCommand communicateAreaUpdate)
        {
        await using var context = await _factory.CreateDbContextAsync();

        // get communicateArea
        var communicateArea = await context.CommunicateAreas.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateAreaUpdate.CommunicateId && c.AreaId == communicateAreaUpdate.AreaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("CommunicateArea not found");
            return new ServiceResponse(false, "Komunikat dla obszaru nie został znaleziony");
        }
        if (communicateArea.IsDeleted)
        {
            _logger.LogWarning("CommunicateArea already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla obszaru został już oznaczony jako usunięty");
        }

        communicateArea.IsDeleted = true;
        communicateArea.UpdatedBy = communicateAreaUpdate.UpdatedBy;
        context.Update(communicateArea);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} marked as deleted", communicateArea.CommunicateId, communicateArea.AreaId);
            return new ServiceResponse(true, "Komunikat dla obszaru został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateArea with id {CommunicateId}, {AreaId} as deleted", communicateArea.CommunicateId, communicateArea.AreaId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla obszaru jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetUpdateCommand communicateAssetUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateAsset = await context.CommunicateAssets.AsNoTracking().FirstOrDefaultAsync(ca => ca.CommunicateId == communicateAssetUpdate.CommunicateId && ca.AssetId == communicateAssetUpdate.AssetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("CommunicateAsset not found");
            return new ServiceResponse(false, "Komunikat dla zasobu nie został znaleziony");
        }
        if (communicateAsset.IsDeleted)
        {
            _logger.LogWarning("CommunicateAsset already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla zasobu został już oznaczony jako usunięty");
        }
        communicateAsset.IsDeleted = true;
        communicateAsset.UpdatedBy = communicateAssetUpdate.UpdatedBy;
        context.Update(communicateAsset);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} marked as deleted", communicateAsset.CommunicateId, communicateAsset.AssetId);
            return new ServiceResponse(true, "Komunikat dla zasobu został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateAsset with id {CommunicateId}, {AssetId} as deleted", communicateAsset.CommunicateId, communicateAsset.AssetId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla zasobu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategoryUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateCategory = await context.CommunicateCategories.AsNoTracking().FirstOrDefaultAsync(cc => cc.CommunicateId == communicateCategoryUpdate.CommunicateId && cc.CategoryId == communicateCategoryUpdate.CategoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("CommunicateCategory not found");
            return new ServiceResponse(false, "Komunikat dla kategorii nie został znaleziony");
        }
        if (communicateCategory.IsDeleted)
        {
            _logger.LogWarning("CommunicateCategory already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla kategorii został już oznaczony jako usunięty");
        }
        communicateCategory.IsDeleted = true;
        communicateCategory.UpdatedBy = communicateCategoryUpdate.UpdatedBy;
        context.Update(communicateCategory);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} marked as deleted", communicateCategory.CommunicateId, communicateCategory.CategoryId);
            return new ServiceResponse(true, "Komunikat dla kategorii został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCategory with id {CommunicateId}, {CategoryId} as deleted", communicateCategory.CommunicateId, communicateCategory.CategoryId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla kategorii jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinateUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateCoordinate = await context.CommunicateCoordinates.AsNoTracking().FirstOrDefaultAsync(cc => cc.CommunicateId == communicateCoordinateUpdate.CommunicateId && cc.CoordinateId == communicateCoordinateUpdate.CoordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("CommunicateCoordinate not found");
            return new ServiceResponse(false, "Komunikat dla koordynatów nie został znaleziony");
        }
        if (communicateCoordinate.IsDeleted)
        {
            _logger.LogWarning("CommunicateCoordinate already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla koordynatów został już oznaczony jako usunięty");
        }
        communicateCoordinate.IsDeleted = true;
        communicateCoordinate.UpdatedBy = communicateCoordinateUpdate.UpdatedBy;
        context.Update(communicateCoordinate);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} marked as deleted", communicateCoordinate.CommunicateId, communicateCoordinate.CoordinateId);
            return new ServiceResponse(true, "Komunikat dla koordynatów został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCoordinate with id {CommunicateId}, {CoordinateId} as deleted", communicateCoordinate.CommunicateId, communicateCoordinate.CoordinateId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla koordynatów jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceUpdateCommand communicateDeviceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateDevice = await context.CommunicateDevices.AsNoTracking().FirstOrDefaultAsync(cd => cd.CommunicateId == communicateDeviceUpdate.CommunicateId && cd.DeviceId == communicateDeviceUpdate.DeviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("CommunicateDevice not found");
            return new ServiceResponse(false, "Komunikat dla urządzenia nie został znaleziony");
        }
        if (communicateDevice.IsDeleted)
        {
            _logger.LogWarning("CommunicateDevice already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla urządzenia został już oznaczony jako usunięty");
        }
        communicateDevice.IsDeleted = true;
        communicateDevice.UpdatedBy = communicateDeviceUpdate.UpdatedBy;
        context.Update(communicateDevice);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} marked as deleted", communicateDevice.CommunicateId, communicateDevice.DeviceId);
            return new ServiceResponse(true, "Komunikat dla urządzenia został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateDevice with id {CommunicateId}, {DeviceId} as deleted", communicateDevice.CommunicateId, communicateDevice.DeviceId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla urządzenia jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelUpdateCommand communicateModelUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateModel = await context.CommunicateModels.AsNoTracking().FirstOrDefaultAsync(cm => cm.CommunicateId == communicateModelUpdate.CommunicateId && cm.ModelId == communicateModelUpdate.ModelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("CommunicateModel not found");
            return new ServiceResponse(false, "Komunikat dla modelu nie został znaleziony");
        }
        if (communicateModel.IsDeleted)
        {
            _logger.LogWarning("CommunicateModel already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla modelu został już oznaczony jako usunięty");
        }
        communicateModel.IsDeleted = true;
        communicateModel.UpdatedBy = communicateModelUpdate.UpdatedBy;
        context.Update(communicateModel);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} marked as deleted", communicateModel.CommunicateId, communicateModel.ModelId);
            return new ServiceResponse(true, "Komunikat dla modelu został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateModel with id {CommunicateId}, {ModelId} as deleted", communicateModel.CommunicateId, communicateModel.ModelId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla modelu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpaceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateSpace = await context.CommunicateSpaces.AsNoTracking().FirstOrDefaultAsync(cs => cs.CommunicateId == communicateSpaceUpdate.CommunicateId && cs.SpaceId == communicateSpaceUpdate.SpaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("CommunicateSpace not found");
            return new ServiceResponse(false, "Komunikat dla strefy nie został znaleziony");
        }
        if (communicateSpace.IsDeleted)
        {
            _logger.LogWarning("CommunicateSpace already marked as deleted");
            return new ServiceResponse(false, "Komunikat dla strefy został już oznaczony jako usunięty");
        }
        communicateSpace.IsDeleted = true;
        communicateSpace.UpdatedBy = communicateSpaceUpdate.UpdatedBy;
        context.Update(communicateSpace);
        try
        {
            // save changes
            await context.SaveChangesAsync();

            _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} marked as deleted", communicateSpace.CommunicateId, communicateSpace.SpaceId);
            return new ServiceResponse(true, "Komunikat dla strefy został oznaczony jako usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateSpace with id {CommunicateId}, {SpaceId} as deleted", communicateSpace.CommunicateId, communicateSpace.SpaceId);

            return new ServiceResponse(false, "Wystąpił błąd podczas oznaczania komunikatu dla strefy jako usuniętego");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicate(CommunicateUpdateCommand communicateUpdateDto)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateUpdateDto.CommunicateId);
        if (communicate == null)
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
       
        // check if duplicate exists
        var exists = await context.Communicates.FirstOrDefaultAsync(c =>
            c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim() &&
            c.CommunicateId != communicateUpdateDto.CommunicateId);
        if (exists != null)
        {
            _logger.LogWarning("Communicate with name {Name} already exists", communicateUpdateDto.Name);
            return new ServiceResponse(false, "Komunikat o podanej nazwie już istnieje");
        }
            communicate.Name = communicateUpdateDto.Name;
            communicate.Description = communicateUpdateDto.Description;
       
        communicate.UpdatedBy = communicateUpdateDto.UpdatedBy;
        communicate.IsDeleted = false;
        context.Communicates.Update(communicate);
        try
        {
            await context.SaveChangesAsync();
            return new ServiceResponse(true, "Komunikat został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicateArea(CommunicateAreaUpdateCommand communicateAreaUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateArea = await context.CommunicateAreas.AsNoTracking().FirstOrDefaultAsync(ca => ca.CommunicateId == communicateAreaUpdate.CommunicateId && ca.AreaId == communicateAreaUpdate.AreaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("CommunicateArea not found");
            return new ServiceResponse(false, "Komunikat dla strefy nie został znaleziony");
        }
        if (communicateArea.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateArea not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla strefy nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateAreaUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate  marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var area = await context.Areas.AsNoTracking().FirstOrDefaultAsync(a => a.AreaId == communicateAreaUpdate.AreaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse(false, "Obszar nie został znaleziony");
        }
        if (area.IsDeleted)
        {
            _logger.LogWarning("Area marked as deleted");
            return new ServiceResponse(false, "Obszar jest oznaczony jako usunięty");
        }

        communicateArea.IsDeleted = false;
        communicateArea.UpdatedBy = communicateAreaUpdate.UpdatedBy;
        context.Update(communicateArea);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} updated", communicateArea.CommunicateId, communicateArea.AreaId);
            return new ServiceResponse(true, "Komunikat dla strefy został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateArea with id {CommunicateId}, {AreaId}", communicateArea.CommunicateId, communicateArea.AreaId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla strefy");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicateAsset(CommunicateAssetUpdateCommand communicateAssetUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateAsset = await context.CommunicateAssets.AsNoTracking().FirstOrDefaultAsync(ca => ca.CommunicateId == communicateAssetUpdate.CommunicateId && ca.AssetId == communicateAssetUpdate.AssetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("CommunicateAsset not found");
            return new ServiceResponse(false, "Komunikat dla zasobu nie został znaleziony");
        }
        if (communicateAsset.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateAsset not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla zasobu nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateAssetUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var asset = await context.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.AssetId == communicateAssetUpdate.AssetId);
        if (asset == null)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse(false, "Zasób nie został znaleziony");
        }
        if (asset.IsDeleted)
        {
            _logger.LogWarning("Asset marked as deleted");
            return new ServiceResponse(false, "Zasób jest oznaczony jako usunięty");
        }
        communicateAsset.IsDeleted = false;
        communicateAsset.UpdatedBy = communicateAssetUpdate.UpdatedBy;
        context.Update(communicateAsset);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} updated", communicateAsset.CommunicateId, communicateAsset.AssetId);
            return new ServiceResponse(true, "Komunikat dla zasobu został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateAsset with id {CommunicateId}, {AssetId}", communicateAsset.CommunicateId, communicateAsset.AssetId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla zasobu");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicateCategory(CommunicateCategoryUpdateCommand communicateCategoryUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateCategory = await context.CommunicateCategories.AsNoTracking().FirstOrDefaultAsync(cc => cc.CommunicateId == communicateCategoryUpdate.CommunicateId && cc.CategoryId == communicateCategoryUpdate.CategoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("CommunicateCategory not found");
            return new ServiceResponse(false, "Komunikat dla kategorii nie został znaleziony");
        }
        if (communicateCategory.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateCategory is not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla kategorii nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateCategoryUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == communicateCategoryUpdate.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse(false, "Kategoria nie została znaleziona");
        }
        if (category.IsDeleted)
        {
            _logger.LogWarning("Category marked as deleted");
            return new ServiceResponse(false, "Kategoria jest oznaczona jako usunięta");
        }
        communicateCategory.IsDeleted = false;
        communicateCategory.UpdatedBy = communicateCategoryUpdate.UpdatedBy;
        context.Update(communicateCategory);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} updated", communicateCategory.CommunicateId, communicateCategory.CategoryId);
            return new ServiceResponse(true, "Komunikat dla kategorii został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateCategory with id {CommunicateId}, {CategoryId}", communicateCategory.CommunicateId, communicateCategory.CategoryId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla kategorii");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicateCoordinate(CommunicateCoordinateUpdateCommand communicateCoordinateUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateCoordinate = await context.CommunicateCoordinates.AsNoTracking().FirstOrDefaultAsync(cc => cc.CommunicateId == communicateCoordinateUpdate.CommunicateId && cc.CoordinateId == communicateCoordinateUpdate.CoordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("CommunicateCoordinate not found");
            return new ServiceResponse(false, "Komunikat dla koordynatu nie został znaleziony");
        }
        if (communicateCoordinate.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateCoordinate is not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla koordynatu nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateCoordinateUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var coordinate = await context.Coordinates.AsNoTracking().FirstOrDefaultAsync(c => c.CoordinateId == communicateCoordinateUpdate.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse(false, "Koordynat nie został znaleziony");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate marked as deleted");
            return new ServiceResponse(false, "Koordynat jest oznaczony jako usunięty");
        }
        communicateCoordinate.IsDeleted = false;
        communicateCoordinate.UpdatedBy = communicateCoordinateUpdate.UpdatedBy;
        context.Update(communicateCoordinate);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} updated", communicateCoordinate.CommunicateId, communicateCoordinate.CoordinateId);
            return new ServiceResponse(true, "Komunikat dla koordynatu został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateCoordinate with id {CommunicateId}, {CoordinateId}", communicateCoordinate.CommunicateId, communicateCoordinate.CoordinateId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla koordynatu");
        }
    }

    public async Task<ServiceResponse> UpdateCommunicateDevice(CommunicateDeviceUpdateCommand communicateDeviceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateDevice = await context.CommunicateDevices.AsNoTracking().FirstOrDefaultAsync(cd => cd.CommunicateId == communicateDeviceUpdate.CommunicateId && cd.DeviceId == communicateDeviceUpdate.DeviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("CommunicateDevice not found");
            return new ServiceResponse(false, "Komunikat dla urządzenia nie został znaleziony");
        }
        if (communicateDevice.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateDevice is not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla urządzenia nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateDeviceUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var device = await context.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == communicateDeviceUpdate.DeviceId);
        if (device == null)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse(false, "Urządzenie nie zostało znalezione");
        }
        if (device.IsDeleted)
        {
            _logger.LogWarning("Device marked as deleted");
            return new ServiceResponse(false, "Urządzenie jest oznaczone jako usunięte");
        }
        communicateDevice.IsDeleted = false;
        communicateDevice.UpdatedBy = communicateDeviceUpdate.UpdatedBy;
        context.Update(communicateDevice);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} updated", communicateDevice.CommunicateId, communicateDevice.DeviceId);
            return new ServiceResponse(true, "Komunikat dla urządzenia został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateDevice with id {CommunicateId}, {DeviceId}", communicateDevice.CommunicateId, communicateDevice.DeviceId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla urządzenia");
        }
    }
    

    public async Task<ServiceResponse> UpdateCommunicateModel(CommunicateModelUpdateCommand communicateModelUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateModel = await context.CommunicateModels.AsNoTracking().FirstOrDefaultAsync(cm => cm.CommunicateId == communicateModelUpdate.CommunicateId && cm.ModelId == communicateModelUpdate.ModelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("CommunicateModel not found");
            return new ServiceResponse(false, "Komunikat dla modelu nie został znaleziony");
        }
        if (communicateModel.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateModel is not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla modelu nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateModelUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var model = await context.Models.AsNoTracking().FirstOrDefaultAsync(m => m.ModelId == communicateModelUpdate.ModelId);
        if (model == null)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse(false, "Model nie został znaleziony");
        }
        if (model.IsDeleted)
        {
            _logger.LogWarning("Model marked as deleted");
            return new ServiceResponse(false, "Model jest oznaczony jako usunięty");
        }
        communicateModel.IsDeleted = false;
        communicateModel.UpdatedBy = communicateModelUpdate.UpdatedBy;
        context.Update(communicateModel);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} updated", communicateModel.CommunicateId, communicateModel.ModelId);
            return new ServiceResponse(true, "Komunikat dla modelu został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateModel with id {CommunicateId}, {ModelId}", communicateModel.CommunicateId, communicateModel.ModelId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla modelu");
        }
    }
    

    public async Task<ServiceResponse> UpdateCommunicateSpace(CommunicateSpaceUpdateCommand communicateSpaceUpdate)
    {
        await using var context = await _factory.CreateDbContextAsync();
        var communicateSpace = await context.CommunicateSpaces.AsNoTracking().FirstOrDefaultAsync(cs => cs.CommunicateId == communicateSpaceUpdate.CommunicateId && cs.SpaceId == communicateSpaceUpdate.SpaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("CommunicateSpace not found");
            return new ServiceResponse(false, "Komunikat dla strefy nie został znaleziony");
        }
        if (communicateSpace.IsDeleted == false)
        {
            _logger.LogWarning("CommunicateSpace is not marked as deleted");
            return new ServiceResponse(false, "Komunikat dla strefy nie został oznaczony jako usunięty");
        }
        var communicate = await context.Communicates.AsNoTracking().FirstOrDefaultAsync(c => c.CommunicateId == communicateSpaceUpdate.CommunicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse(false, "Komunikat nie został znaleziony");
        }
        if (communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate marked as deleted");
            return new ServiceResponse(false, "Komunikat jest oznaczony jako usunięty");
        }
        var space = await context.Spaces.AsNoTracking().FirstOrDefaultAsync(s => s.SpaceId == communicateSpaceUpdate.SpaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse(false, "Strefa nie została znaleziona");
        }
        if (space.IsDeleted)
        {
            _logger.LogWarning("Space marked as deleted");
            return new ServiceResponse(false, "Strefa jest oznaczona jako usunięta");
        }
        communicateSpace.IsDeleted = false;
        communicateSpace.UpdatedBy = communicateSpaceUpdate.UpdatedBy;
        context.Update(communicateSpace);
        try
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} updated", communicateSpace.CommunicateId, communicateSpace.SpaceId);
            return new ServiceResponse(true, "Komunikat dla strefy został zaktualizowany");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicateSpace with id {CommunicateId}, {SpaceId}", communicateSpace.CommunicateId, communicateSpace.SpaceId);

            return new ServiceResponse(false, "Wystąpił błąd podczas aktualizacji komunikatu dla strefy");
        }
    }
}