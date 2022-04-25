using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.DTO;
using Sc3S.Entities;
using Sc3S.Exceptions;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface ICommunicateService
{
    Task<ServiceResponse<int>> CreateCommunicate(CommunicateUpdateCommand communicateUpdateDto);

    Task<ServiceResponse<(int,int)>> CreateCommunicateArea(int communicateId, int areaId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateAsset(int communicateId, int assetId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateCategory(int communicateId, int categoryId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateCoordinate(int communicateId, int coordinateId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateDevice(int communicateId, int deviceId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateModel(int communicateId, int modelId);

    Task<ServiceResponse<(int,int)>> CreateCommunicateSpace(int communicateId, int spaceId);

    Task<ServiceResponse>DeleteCommunicate(int communicateId);

    Task<ServiceResponse>DeleteCommunicateArea(int communicateId, int areaId);

    Task<ServiceResponse>DeleteCommunicateAsset(int communicateId, int assetId);

    Task<ServiceResponse>DeleteCommunicateCategory(int communicateId, int categoryId);

    Task<ServiceResponse>DeleteCommunicateCoordinate(int communicateId, int coordinateId);

    Task<ServiceResponse>DeleteCommunicateDevice(int communicateId, int deviceId);

    Task<ServiceResponse>DeleteCommunicateModel(int communicateId, int modelId);

    Task<ServiceResponse>DeleteCommunicateSpace(int communicateId, int spaceId);

    Task<CommunicateQuery> GetCommunicateById(int communicateId);

    Task<IEnumerable<CommunicateQuery>> GetCommunicates();

    Task<IEnumerable<CommunicateWithAssetsDto>> GetCommunicatesWithAssets();

    Task<ServiceResponse>MarkDeleteCommunicate(int communicateId);

    Task<ServiceResponse>MarkDeleteCommunicateArea(int communicateId, int areaId);

    Task<ServiceResponse>MarkDeleteCommunicateAsset(int communicateId, int assetId);

    Task<ServiceResponse>MarkDeleteCommunicateCategory(int communicateId, int categoryId);

    Task<ServiceResponse>MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId);

    Task<ServiceResponse>MarkDeleteCommunicateDevice(int communicateId, int deviceId);

    Task<ServiceResponse>MarkDeleteCommunicateModel(int communicateId, int modelId);

    Task<ServiceResponse>MarkDeleteCommunicateSpace(int communicateId, int spaceId);

    Task<ServiceResponse>UpdateCommunicate(int communicateId, CommunicateUpdateCommand communicateUpdateDto);

    Task<ServiceResponse>UpdateCommunicateArea(int communicateId, int areaId);
    Task<ServiceResponse>UpdateCommunicateAsset(int communicateId, int assetId);
    Task<ServiceResponse>UpdateCommunicateCategory(int communicateId, int categoryId);
    Task<ServiceResponse>UpdateCommunicateCoordinate(int communicateId, int coordinateId);
    Task<ServiceResponse>UpdateCommunicateDevice(int communicateId, int deviceId);
    Task<ServiceResponse>UpdateCommunicateModel(int communicateId, int modelId);
    Task<ServiceResponse>UpdateCommunicateSpace(int communicateId, int spaceId);
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

        await using var _context = await _factory.CreateDbContextAsync();


        // validate category name
        var duplicate = await _context.Communicates.AnyAsync(c => c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Communicate name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa komunikatu jest zajęta");
        }

        var communicate = new Communicate
        {

            Name = communicateUpdateDto.Name,
            Description = communicateUpdateDto.Description,
            IsDeleted = false
        };
        // create category
        _context.Communicates.Add(communicate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate with id {CommunicateId} created", communicate.CommunicateId);

            return new ServiceResponse<int>(true, communicate.CommunicateId, "Komunikat stworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating communicate");


            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia komunikatu");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateArea(int communicateId, int areaId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateArea
        var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego obszaru");
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Obszar nie został znaleziony");
        }
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateArea = new CommunicateArea
        {
            CommunicateId = communicateId,
            AreaId = areaId,

            IsDeleted = false
        };
        _context.Add(communicateArea);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, areaId), "Komunikat został przypisany do obszaru");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateArea");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateAsset(int communicateId, int assetId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateAsset
        var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego zasobu");
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Zasób nie został znaleziony");
        }
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        communicateAsset = new CommunicateAsset
        {
            CommunicateId = communicateId,
            AssetId = assetId,

            IsDeleted = false
        };
        _context.Add(communicateAsset);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, assetId), "Komunikat został przypisany do zasobu");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateAsset");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateCategory(int communicateId, int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateCategory
        var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tej kategorii");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Kategoria nie została znaleziona");
        }
        communicateCategory = new CommunicateCategory
        {
            CommunicateId = communicateId,
            CategoryId = categoryId,

            IsDeleted = false
        };
        _context.Add(communicateCategory);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, categoryId), "Komunikat został przypisany do kategorii");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateCategory");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do kategorii");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateCoordinate(int communicateId, int coordinateId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateCoordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tej koordynaty");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Koordynat nie został znaleziony");
        }
        communicateCoordinate = new CommunicateCoordinate
        {
            CommunicateId = communicateId,
            CoordinateId = coordinateId,

            IsDeleted = false
        };
        _context.Add(communicateCoordinate);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, coordinateId), "Komunikat został przypisany do koordynatu");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateCoordinate");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do koordynatu");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateDevice(int communicateId, int deviceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateDevice
        var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego urządzenia");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Urządzenie nie zostało znalezione");
        }
        communicateDevice = new CommunicateDevice
        {
            CommunicateId = communicateId,
            DeviceId = deviceId,

            IsDeleted = false
        };
        _context.Add(communicateDevice);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, deviceId), "Komunikat został przypisany do urządzenia");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateDevice");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do urządzenia");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateModel(int communicateId, int modelId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateModel
        var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego modelu");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        var model = await _context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Model nie został znaleziony");
        }
        communicateModel = new CommunicateModel
        {
            CommunicateId = communicateId,
            ModelId = modelId,

            IsDeleted = false
        };
        _context.Add(communicateModel);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, modelId), "Komunikat został przypisany do modelu");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateModel");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do modelu");
        }
    }

    public async Task<ServiceResponse<(int,int)>> CreateCommunicateSpace(int communicateId, int spaceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateSpace
        var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace != null)
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat już jest przypisany do tego miejsca");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Komunikat nie został znaleziony");
        }
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Miejsce nie zostało znalezione");
        }
        communicateSpace = new CommunicateSpace
        {
            CommunicateId = communicateId,
            SpaceId = spaceId,

            IsDeleted = false
        };
        _context.Add(communicateSpace);
        // save changes

        try
        {
            await _context.SaveChangesAsync();

            return new ServiceResponse<(int, int)>(true, (communicateId, spaceId), "Komunikat został przypisany do miejsca");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error creating communicateSpace");
            return new ServiceResponse<(int, int)>(false, (-1, -1), "Błąd podczas przypisywania komunikatu do miejsca");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicate(int communicateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicate
        var communicate = await _context.Communicates.FindAsync(communicateId);
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
        _context.Communicates.Remove(communicate);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate with id {CommunicateId} deleted", communicate.CommunicateId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate");


            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateArea(int communicateId, int areaId)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        // get communicate area
        var communicateArea = await _context.CommunicateAreas.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AreaId == areaId);
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
        _context.CommunicateAreas.Remove(communicateArea);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate area with id {CommunicateId}, {AreaId}  deleted", communicateId, areaId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate area");


            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateAsset(int communicateId, int assetId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicate asset
        var communicateAsset = await _context.CommunicateAssets.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.AssetId == assetId);
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
        _context.CommunicateAssets.Remove(communicateAsset);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate asset with id {CommunicateId}, {AssetId}  deleted", communicateId,assetId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate asset");


            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateCategory(int communicateId, int categoryId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicate category
        var communicateCategory = await _context.CommunicateCategories.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CategoryId == categoryId);
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
        _context.CommunicateCategories.Remove(communicateCategory);


        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate category with id {CommunicateId}, {CategoryId}  deleted", communicateId, categoryId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate category");


            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();



        // get communicate coordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.CoordinateId == coordinateId);
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
        _context.CommunicateCoordinates.Remove(communicateCoordinate);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate coordinate with id {CommunicateId}, {CoordinateId}  deleted", communicateId, coordinateId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate coordinate");


            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateDevice(int communicateId, int deviceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();



        // get communicate device
        var communicateDevice = await _context.CommunicateDevices.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.DeviceId == deviceId);
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
        _context.CommunicateDevices.Remove(communicateDevice);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate device with id {CommunicateId}, {DeviceId}  deleted", communicateId, deviceId);
            return new ServiceResponse(true, "Komunikat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate device");
            return new ServiceResponse(false, "Błąd podczas usuwania komunikatu");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateModel(int communicateId, int modelId)
    {
        await using var _context = await _factory.CreateDbContextAsync();



        // get communicate model
        var communicateModel = await _context.CommunicateModels.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.ModelId == modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("Communicate model not found");
            return new ServiceResponse("Communicate model not found");
        }
        // check if CommunicateModel is not marked as deleted
        if (communicateModel.IsDeleted == false)
        {
            _logger.LogWarning("Communicate model is not marked as deleted");
            return new ServiceResponse("Communicate model is not marked as deleted");
        }
        // delete communicate model
        _context.CommunicateModels.Remove(communicateModel);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate model with id {CommunicateId}, {ModelId}  deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate model");


            return new ServiceResponse("Error deleting communicate model");
        }
    }

    public async Task<ServiceResponse>DeleteCommunicateSpace(int communicateId, int spaceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();



        // get communicate space
        var communicateSpace = await _context.CommunicateSpaces.FirstOrDefaultAsync(c => c.CommunicateId == communicateId && c.SpaceId == spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("Communicate space not found");
            return new ServiceResponse("Communicate space not found");
        }
        // check if CommunicateSpace is not marked as deleted
        if (communicateSpace.IsDeleted == false)
        {
            _logger.LogWarning("Communicate space is not marked as deleted");
            return new ServiceResponse("Communicate space is not marked as deleted");
        }
        // delete communicate space
        _context.CommunicateSpaces.Remove(communicateSpace);

        try
        {
            // save changes
            await _context.SaveChangesAsync();


            _logger.LogInformation("Communicate space with id {CommunicateId}, {SpaceId}  deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting communicate space");


            return new ServiceResponse("Error deleting communicate space");
        }
    }

    public async Task<CommunicateQuery> GetCommunicateById(int communicateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicate
        var communicate = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateQuery
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy

            })
            .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
        if (communicate == null)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        // return communicate
        _logger.LogInformation("Communicate with id {CommunicateId} returned", communicateId);
        return communicate;
    }

    public async Task<IEnumerable<CommunicateQuery>> GetCommunicates()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicates
        var communicates = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateQuery
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            return new ServiceResponse("Communicates not found");
        }

        // return communicates
        _logger.LogInformation("Communicates returned");
        return communicates;
    }

    public async Task<IEnumerable<CommunicateWithAssetsDto>> GetCommunicatesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get communicates
        var communicates = await _context.Communicates
            .AsNoTracking()
            .Select(c => new CommunicateWithAssetsDto
            {
                CommunicateId = c.CommunicateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UserId = c.UpdatedBy,
                Assets = c.CommunicateAssets.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UpdatedBy
                }).ToList()
            })
            .ToListAsync();
        if (communicates is null)
        {
            _logger.LogWarning("Communicates not found");
            return new ServiceResponse("Communicates not found");
        }
        // return communicates
        _logger.LogInformation("Communicates returned");
        return communicates;
    }

    public async Task<SituationQuery> GetSituationById(int situationId)
    {
        await using var _context = await _factory.CreateDbContextAsync();


        // get situation
        var situation = await _context.Situations
            .AsNoTracking()
            .Select(s => new SituationQuery
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UpdatedBy
            })
            .FirstOrDefaultAsync(c => c.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task<ServiceResponse>MarkDeleteCommunicate(int communicateId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicate
            var communicate = await _context.Communicates
                .Include(c => c.CommunicateAssets)
                .FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            // if communicate not found
            if (communicate == null)
            {
                _logger.LogWarning("Communicate with id {CommunicateId} not found", communicateId);
                return new ServiceResponse($"Communicate with id {communicateId} not found");
            }
            // if communicate is already deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate with id {CommunicateId} is already deleted", communicateId);
                return new ServiceResponse($"Communicate with id {communicateId} is already deleted");
            }
            // check if communicate has CommunicateAssets with IsDeleted = false
            if (communicate.CommunicateAssets.Any(ca => ca.IsDeleted == false))
            {
                _logger.LogWarning("Communicate with id {CommunicateId} has CommunicateAssets with IsDeleted = false", communicateId);
                return new ServiceResponse($"Communicate with id {communicateId} has CommunicateAssets with IsDeleted = false");
            }

            // mark communicate as deleted
            communicate.IsDeleted = true;

            // save changes
            await _context.SaveChangesAsync();


            // return success
            _logger.LogInformation("Communicate with id {CommunicateId} marked as deleted", communicateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicate with id {CommunicateId} as deleted", communicateId);


            return new ServiceResponse($"Error marking communicate with id {communicateId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateArea(int communicateId, int areaId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateArea
            var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
            if (communicateArea == null)
            {
                _logger.LogWarning("CommunicateArea not found");
                return new ServiceResponse("CommunicateArea not found");
            }
            if (communicateArea.IsDeleted)
            {
                _logger.LogWarning("CommunicateArea already marked as deleted");
                return new ServiceResponse("CommunicateArea already marked as deleted");
            }

            communicateArea.IsDeleted = true;
            _context.Update(communicateArea);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateArea with id {CommunicateId}, {AreaId} marked as deleted", communicateId, areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateArea with id {CommunicateId}, {AreaId} as deleted", communicateId, areaId);

            return new ServiceResponse($"Error marking communicateArea with id {communicateId}, {areaId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateAsset(int communicateId, int assetId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateAsset
            var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
            if (communicateAsset == null)
            {
                _logger.LogWarning("CommunicateAsset not found");
                return new ServiceResponse("CommunicateAsset not found");
            }
            if (communicateAsset.IsDeleted)
            {
                _logger.LogWarning("CommunicateAsset already marked as deleted");
                return new ServiceResponse("CommunicateAsset already marked as deleted");
            }

            communicateAsset.IsDeleted = true;
            _context.Update(communicateAsset);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateAsset with id {CommunicateId}, {AssetId} marked as deleted", communicateId, assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateAsset with id {CommunicateId}, {AssetId} as deleted", communicateId, assetId);

            return new ServiceResponse($"Error marking communicateAsset with id {communicateId}, {assetId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateCategory(int communicateId, int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateCategory
            var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
            if (communicateCategory == null)
            {
                _logger.LogWarning("CommunicateCategory not found");
                return new ServiceResponse("CommunicateCategory not found");
            }
            if (communicateCategory.IsDeleted)
            {
                _logger.LogWarning("CommunicateCategory already marked as deleted");
                return new ServiceResponse("CommunicateCategory already marked as deleted");
            }

            communicateCategory.IsDeleted = true;
            _context.Update(communicateCategory);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateCategory with id {CommunicateId}, {CategoryId} marked as deleted", communicateId, categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCategory with id {CommunicateId}, {CategoryId} as deleted", communicateId, categoryId);

            return new ServiceResponse($"Error marking communicateCategory with id {communicateId}, {categoryId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateCoordinate
            var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
            if (communicateCoordinate == null)
            {
                _logger.LogWarning("CommunicateCoordinate not found");
                return new ServiceResponse("CommunicateCoordinate not found");
            }
            if (communicateCoordinate.IsDeleted)
            {
                _logger.LogWarning("CommunicateCoordinate already marked as deleted");
                return new ServiceResponse("CommunicateCoordinate already marked as deleted");
            }

            communicateCoordinate.IsDeleted = true;
            _context.Update(communicateCoordinate);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateCoordinate with id {CommunicateId}, {CoordinateId} marked as deleted", communicateId, coordinateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateCoordinate with id {CommunicateId}, {CoordinateId} as deleted", communicateId, coordinateId);

            return new ServiceResponse($"Error marking communicateCoordinate with id {communicateId}, {coordinateId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateDevice(int communicateId, int deviceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateDevice
            var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
            if (communicateDevice == null)
            {
                _logger.LogWarning("CommunicateDevice not found");
                return new ServiceResponse("CommunicateDevice not found");
            }
            if (communicateDevice.IsDeleted)
            {
                _logger.LogWarning("CommunicateDevice already marked as deleted");
                return new ServiceResponse("CommunicateDevice already marked as deleted");
            }

            communicateDevice.IsDeleted = true;
            _context.Update(communicateDevice);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateDevice with id {CommunicateId}, {DeviceId} marked as deleted", communicateId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateDevice with id {CommunicateId}, {DeviceId} as deleted", communicateId, deviceId);

            return new ServiceResponse($"Error marking communicateDevice with id {communicateId}, {deviceId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateModel(int communicateId, int modelId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateModel
            var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
            if (communicateModel == null)
            {
                _logger.LogWarning("CommunicateModel not found");
                return new ServiceResponse("CommunicateModel not found");
            }
            if (communicateModel.IsDeleted)
            {
                _logger.LogWarning("CommunicateModel already marked as deleted");
                return new ServiceResponse("CommunicateModel already marked as deleted");
            }

            communicateModel.IsDeleted = true;
            _context.Update(communicateModel);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateModel with id {CommunicateId}, {ModelId} marked as deleted", communicateId, modelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateModel with id {CommunicateId}, {ModelId} as deleted", communicateId, modelId);

            return new ServiceResponse($"Error marking communicateModel with id {communicateId}, {modelId} as deleted");
        }
    }

    public async Task<ServiceResponse>MarkDeleteCommunicateSpace(int communicateId, int spaceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        try
        {
            // get communicateSpace
            var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
            if (communicateSpace == null)
            {
                _logger.LogWarning("CommunicateSpace not found");
                return new ServiceResponse("CommunicateSpace not found");
            }
            if (communicateSpace.IsDeleted)
            {
                _logger.LogWarning("CommunicateSpace already marked as deleted");
                return new ServiceResponse("CommunicateSpace already marked as deleted");
            }

            communicateSpace.IsDeleted = true;
            _context.Update(communicateSpace);
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("CommunicateSpace with id {CommunicateId}, {SpaceId} marked as deleted", communicateId, spaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking communicateSpace with id {CommunicateId}, {SpaceId} as deleted", communicateId, spaceId);

            return new ServiceResponse($"Error marking communicateSpace with id {communicateId}, {spaceId} as deleted");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicate(int communicateId, CommunicateUpdateCommand communicateUpdateDto)
    {

        await using var _context = await _factory.CreateDbContextAsync();



        // try-catch
        try
        {
            var communicate = await _context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            if (communicate == null)
                return new ServiceResponse("Communicate not found");
            // check if communicate is not marked as deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate is marked as deleted");
                return new ServiceResponse("Communicate is marked as deleted");
            }
            // check if duplicate exists and is not marked as deleted
            var exists = await _context.Communicates.AnyAsync(c =>
                c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CommunicateId != communicateId);
            if (exists)
            {
                _logger.LogWarning("Communicate with name {Name} already exists", communicateUpdateDto.Name);
                return new ServiceResponse($"Communicate with name {communicateUpdateDto.Name} already exists");
            }

            // update name
            if (!Equals(communicateUpdateDto.Name.ToLower().Trim(), communicate.Name.ToLower().Trim()))
            {
                communicate.Name = communicateUpdateDto.Name;
            }

            if (!Equals(communicateUpdateDto.Description.ToLower().Trim(), communicate.Description.ToLower().Trim()))
            {
                communicate.Description = communicateUpdateDto.Description;
            }

            _context.Communicates.Update(communicate);
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");

            return new ServiceResponse("Error updating communicate");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateArea(int communicateId, int areaId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateArea
        var communicateArea = await _context.CommunicateAreas.FindAsync(communicateId, areaId);
        if (communicateArea == null)
        {
            _logger.LogWarning("CommunicateArea not found");
            return new ServiceResponse("CommunicateArea not found");
        }
        if (!communicateArea.IsDeleted)
            return new ServiceResponse("CommunicateArea not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse("Area not found");
        }
        communicateArea.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateArea");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateAsset(int communicateId, int assetId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateAsset
        var communicateAsset = await _context.CommunicateAssets.FindAsync(communicateId, assetId);
        if (communicateAsset == null)
        {
            _logger.LogWarning("CommunicateAsset not found");
            return new ServiceResponse("CommunicateAsset not found");
        }
        if (!communicateAsset.IsDeleted)
            return new ServiceResponse("CommunicateAsset not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || asset.IsDeleted)
        {
            _logger.LogWarning("Asset not found");
            return new ServiceResponse("Asset not found");
        }
        communicateAsset.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateAsset");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateCategory(int communicateId, int categoryId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateCategory
        var communicateCategory = await _context.CommunicateCategories.FindAsync(communicateId, categoryId);
        if (communicateCategory == null)
        {
            _logger.LogWarning("CommunicateCategory not found");
            return new ServiceResponse("CommunicateCategory not found");
        }
        if (!communicateCategory.IsDeleted)
            return new ServiceResponse("CommunicateCategory not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null || category.IsDeleted)
        {
            _logger.LogWarning("Category not found");
            return new ServiceResponse("Category not found");
        }
        communicateCategory.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateCategory");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateCoordinate(int communicateId, int coordinateId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateCoordinate
        var communicateCoordinate = await _context.CommunicateCoordinates.FindAsync(communicateId, coordinateId);
        if (communicateCoordinate == null)
        {
            _logger.LogWarning("CommunicateCoordinate not found");
            return new ServiceResponse("CommunicateCoordinate not found");
        }
        if (!communicateCoordinate.IsDeleted)
            return new ServiceResponse("CommunicateCoordinate not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null || coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse("Coordinate not found");
        }
        communicateCoordinate.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateCoordinate");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateDevice(int communicateId, int deviceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateDevice
        var communicateDevice = await _context.CommunicateDevices.FindAsync(communicateId, deviceId);
        if (communicateDevice == null)
        {
            _logger.LogWarning("CommunicateDevice not found");
            return new ServiceResponse("CommunicateDevice not found");
        }
        if (!communicateDevice.IsDeleted)
            return new ServiceResponse("CommunicateDevice not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var device = await _context.Devices.FindAsync(deviceId);
        if (device == null || device.IsDeleted)
        {
            _logger.LogWarning("Device not found");
            return new ServiceResponse("Device not found");
        }
        communicateDevice.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateDevice");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateModel(int communicateId, int modelId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateModel
        var communicateModel = await _context.CommunicateModels.FindAsync(communicateId, modelId);
        if (communicateModel == null)
        {
            _logger.LogWarning("CommunicateModel not found");
            return new ServiceResponse("CommunicateModel not found");
        }
        if (!communicateModel.IsDeleted)
            return new ServiceResponse("CommunicateModel not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var model = await _context.Models.FindAsync(modelId);
        if (model == null || model.IsDeleted)
        {
            _logger.LogWarning("Model not found");
            return new ServiceResponse("Model not found");
        }
        communicateModel.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateModel");
            return new ServiceResponse("Error while saving changes");
        }
    }

    public async Task<ServiceResponse>UpdateCommunicateSpace(int communicateId, int spaceId)
    {

        await using var _context = await _factory.CreateDbContextAsync();

        // get communicateSpace
        var communicateSpace = await _context.CommunicateSpaces.FindAsync(communicateId, spaceId);
        if (communicateSpace == null)
        {
            _logger.LogWarning("CommunicateSpace not found");
            return new ServiceResponse("CommunicateSpace not found");
        }
        if (!communicateSpace.IsDeleted)
            return new ServiceResponse("CommunicateSpace not marked as deleted");
        var communicate = await _context.Communicates.FindAsync(communicateId);
        if (communicate == null || communicate.IsDeleted)
        {
            _logger.LogWarning("Communicate not found");
            return new ServiceResponse("Communicate not found");
        }
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null || space.IsDeleted)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse("Space not found");
        }
        communicateSpace.IsDeleted = false;
        // save changes

        try
        {
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error updating communicateSpace");
            return new ServiceResponse("Error while saving changes");
        }
    }

}
