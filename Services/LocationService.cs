using Microsoft.EntityFrameworkCore;

using Sc3S.CQRS.Commands;
using Sc3S.CQRS.Queries;
using Sc3S.Data;
using Sc3S.Entities;
using Sc3S.Helpers;

namespace Sc3S.Services;

public interface ILocationService
{
    Task<ServiceResponse<int>> CreateArea(AreaUpdateCommand areaUpdateDto);

    Task<ServiceResponse<int>> CreateCoordinate(CoordinateUpdateCommand coordinateUpdateDto);

    Task<ServiceResponse<int>> CreatePlant(PlantUpdateCommand plantUpdateDto);

    Task<ServiceResponse<int>> CreateSpace(SpaceUpdateCommand spaceUpdateDto);

    Task<ServiceResponse> DeleteArea(int areaId);

    Task<ServiceResponse> DeleteCoordinate(int coordinateId);

    Task<ServiceResponse> DeletePlant(int plantId);

    Task<ServiceResponse> DeleteSpace(int spaceId);

    Task<ServiceResponse<AreaQuery>> GetAreaById(int areaId);

    Task<ServiceResponse<IEnumerable<AreaQuery>>> GetAreas();

    Task<ServiceResponse<IEnumerable<AreaQuery>>> GetAreasWithSpaces();

    Task<ServiceResponse<CoordinateQuery>> GetCoordinateByIdWithAssets(int coordinateId);

    Task<ServiceResponse<IEnumerable<CoordinateQuery>>> GetCoordinates();

    Task<ServiceResponse<IEnumerable<CoordinateQuery>>> GetCoordinatesWithAssets();

    Task<ServiceResponse<PlantQuery>> GetPlantById(int plantId);

    Task<ServiceResponse<IEnumerable<PlantQuery>>> GetPlants();

    Task<ServiceResponse<IEnumerable<PlantQuery>>> GetPlantsWithAreas();

    Task<ServiceResponse<SpaceQuery>> GetSpaceById(int spaceId);

    Task<ServiceResponse<IEnumerable<SpaceQuery>>> GetSpaces();

    Task<ServiceResponse<IEnumerable<SpaceQuery>>> GetSpacesWithCoordinates();

    Task<ServiceResponse> MarkDeleteArea(AreaUpdateCommand areaUpdate);

    Task<ServiceResponse> MarkDeleteCoordinate(CoordinateUpdateCommand coordinateUpdate);

    Task<ServiceResponse> MarkDeletePlant(PlantUpdateCommand plantUpdate);

    Task<ServiceResponse> MarkDeleteSpace(SpaceUpdateCommand spaceUpdate);

    Task<ServiceResponse> UpdateArea(AreaUpdateCommand areaUpdateDto);

    Task<ServiceResponse> UpdateCoordinate(CoordinateUpdateCommand coordinateUpdateDto);

    Task<ServiceResponse> UpdatePlant(PlantUpdateCommand plantUpdateDto);

    Task<ServiceResponse> UpdateSpace(SpaceUpdateCommand spaceUpdateDto);
}

public class LocationService : ILocationService
{
    private readonly IDbContextFactory<Sc3SContext> _factory;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IDbContextFactory<Sc3SContext> factory, ILogger<LocationService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ServiceResponse<int>> CreateArea(AreaUpdateCommand areaUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plant
        var plant = await _context.Plants
            .AsNoTracking()
            .Include(p => p.Areas)
            .FirstOrDefaultAsync(p => p.PlantId == areaUpdateDto.PlantId);
        if (plant is null || plant.IsDeleted)
        {
            return new ServiceResponse<int>(false, -1, "Nie znaleziono fabryki");
        }
        if (plant.Areas.Any(a => a.Name.ToLower().Trim() == areaUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Area name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa obszaru już istnieje");
        }

        var area = new Area
        {
            Name = areaUpdateDto.Name,
            Description = areaUpdateDto.Description,
            PlantId = plant.PlantId,
            IsDeleted = false,
            CreatedBy = areaUpdateDto.UpdatedBy,
            UpdatedBy = areaUpdateDto.UpdatedBy
        };
        // create area
        plant.Areas.Add(area);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Area with id {AreaId} created", area.AreaId);
            return new ServiceResponse<int>(true, area.AreaId, "Obszar został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating area");
            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia obszaru");
        }
    }

    public async Task<ServiceResponse<int>> CreateCoordinate(CoordinateUpdateCommand coordinateUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get space
        var space = await _context.Spaces.Include(s => s.Coordinates).AsNoTracking().FirstOrDefaultAsync(s => s.SpaceId == coordinateUpdateDto.SpaceId);
        if (space is null || space.IsDeleted)
        {
            return new ServiceResponse<int>(false, -1, "Nie znaleziono strefy");
        }
        // validate coordinate name

        if (space.Coordinates.Any(c => c.SpaceId == coordinateUpdateDto.SpaceId && c.Name.ToLower().Trim() == coordinateUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Coordinate name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa koordynatu już istnieje");
        }

        var coordinate = new Coordinate
        {
            Name = coordinateUpdateDto.Name,
            Description = coordinateUpdateDto.Description,
            SpaceId = coordinateUpdateDto.SpaceId,
            IsDeleted = false,
            CreatedBy = coordinateUpdateDto.UpdatedBy,
            UpdatedBy = coordinateUpdateDto.UpdatedBy
        };
        // create coordinate
        space.Coordinates.Add(coordinate);
        try
        {
            // save changes
            await _context.SaveChangesAsync();
            _logger.LogInformation("Coordinate with id {CoordinateId} created", coordinate.CoordinateId);
            return new ServiceResponse<int>(true, coordinate.CoordinateId, "Koordynat został utworzony");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coordinate");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia koordynatu");
        }
    }

    public async Task<ServiceResponse<int>> CreatePlant(PlantUpdateCommand plantUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // validate plant name
        var duplicate = await _context.Plants
            .FirstOrDefaultAsync(p => p.Name.ToLower().Trim() == plantUpdateDto.Name.ToLower().Trim());
        if (duplicate is not null)
        {
            _logger.LogWarning("Plant name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa fabryki już istnieje");
        }

        var plant = new Plant
        {
            Name = plantUpdateDto.Name,
            Description = plantUpdateDto.Description,
            IsDeleted = false,
            CreatedBy = plantUpdateDto.UpdatedBy,
            UpdatedBy = plantUpdateDto.UpdatedBy
        };
        // create plant
        _context.Plants.Add(plant);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Plant with id {PlantId} created", plant.PlantId);
            return new ServiceResponse<int>(true, plant.PlantId, "Fabryka została utworzona");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia fabryki");
        }
    }

    public async Task<ServiceResponse<int>> CreateSpace(SpaceUpdateCommand spaceUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // check if area exists
        var area = await _context.Areas.Include(a => a.Spaces).FirstOrDefaultAsync(a => a.AreaId == spaceUpdateDto.AreaId);
        if (area == null || area.IsDeleted)
        {
            _logger.LogWarning("Area with id {AreaId} not found", spaceUpdateDto.AreaId);
            return new ServiceResponse<int>(false, -1, "Nie znaleziono obszaru");
        }
        // validate space name
        if (area.Spaces.Any(s => s.AreaId == spaceUpdateDto.AreaId && s.Name.ToLower().Trim() == spaceUpdateDto.Name.ToLower().Trim()))
        {
            _logger.LogWarning("Space name already exists");
            return new ServiceResponse<int>(false, -1, "Nazwa strefy już istnieje");
        }

        var space = new Space
        {
            Name = spaceUpdateDto.Name,
            Description = spaceUpdateDto.Description,
            AreaId = spaceUpdateDto.AreaId,
            SpaceType = spaceUpdateDto.SpaceType,
            IsDeleted = false,
            CreatedBy = spaceUpdateDto.UpdatedBy,
            UpdatedBy = spaceUpdateDto.UpdatedBy
        };
        // create space
        area.Spaces.Add(space);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Space with id {SpaceId} created", space.SpaceId);
            return new ServiceResponse<int>(true, space.SpaceId, "Strefa została utworzona");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating space");

            return new ServiceResponse<int>(false, -1, "Błąd podczas tworzenia strefy");
        }
    }

    public async Task<ServiceResponse> DeleteArea(int areaId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get area
        var area = await _context.Areas.FindAsync(areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse(false, "Nie znaleziono obszaru");
        }
        // check if area is marked as deleted
        if (area.IsDeleted == false)
        {
            _logger.LogWarning("Area not marked as deleted");
            return new ServiceResponse(false, "Obszar nie jest oznaczony jako usunięty");
        }
        _context.Areas.Remove(area);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Area with id {AreaId} deleted", area.AreaId);
            return new ServiceResponse(true, "Obszar został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting area");

            return new ServiceResponse(false, "Błąd podczas usuwania obszaru");
        }
    }

    public async Task<ServiceResponse> DeleteCoordinate(int coordinateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await _context.Coordinates.FindAsync(coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse(false, "Nie znaleziono koordynatu");
        }
        // check if coordinate is marked as deleted
        if (coordinate.IsDeleted == false)
        {
            _logger.LogWarning("Coordinate not marked as deleted");
            return new ServiceResponse(false, "Koordynat nie jest oznaczony jako usunięty");
        }
        _context.Coordinates.Remove(coordinate);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Coordinate with id {CoordinateId} deleted", coordinate.CoordinateId);
            return new ServiceResponse(true, "Koordynat został usunięty");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coordinate");

            return new ServiceResponse(false, "Błąd podczas usuwania koordynatu");
        }
    }

    public async Task<ServiceResponse> DeletePlant(int plantId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plant
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            return new ServiceResponse(false, "Nie znaleziono fabryki");
        }
        // check if plant is marked as deleted
        if (plant.IsDeleted == false)
        {
            _logger.LogWarning("Plant not marked as deleted");
            return new ServiceResponse(false, "Fabryka nie jest oznaczona jako usunięta");
        }
        _context.Plants.Remove(plant);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Plant with id {PlantId} deleted", plant.PlantId);
            return new ServiceResponse(true, "Fabryka została usunięta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");

            return new ServiceResponse(false, "Błąd podczas usuwania fabryki");
        }
    }

    public async Task<ServiceResponse> DeleteSpace(int spaceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get space
        var space = await _context.Spaces.FindAsync(spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse(false, "Nie znaleziono strefy");
        }
        // check if space is marked as deleted
        if (space.IsDeleted == false)
        {
            _logger.LogWarning("Space not marked as deleted");
            return new ServiceResponse(false, "Strefa nie jest oznaczona jako usunięta");
        }
        _context.Spaces.Remove(space);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Space with id {SpaceId} deleted", space.SpaceId);
            return new ServiceResponse(true, "Strefa została usunięta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting space");

            return new ServiceResponse(false, "Błąd podczas usuwania strefy");
        }
    }

    public async Task<ServiceResponse<AreaQuery>> GetAreaById(int areaId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get area
        var area = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaQuery
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                CreatedOn = a.CreatedOn,
                UpdatedOn = a.UpdatedOn,
                UpdatedBy = a.UpdatedBy,
                CreatedBy = a.CreatedBy
            }).FirstOrDefaultAsync(a => a.AreaId == areaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse<AreaQuery>(false, null, "Nie znaleziono obszaru");
        }
        // return area
        _logger.LogInformation("Area with id {AreaId} returned", area.AreaId);
        return new ServiceResponse<AreaQuery>(true, area, "Obszar został zwrócony");
    }

    public async Task<ServiceResponse<IEnumerable<AreaQuery>>> GetAreas()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get areas
        var areas = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaQuery
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UpdatedBy = a.UpdatedBy,
                CreatedBy = a.CreatedBy,
                CreatedOn = a.CreatedOn,
                UpdatedOn = a.UpdatedOn
            }).ToListAsync();
        if (areas is null)
        {
            _logger.LogWarning("No areas found");
            return new ServiceResponse<IEnumerable<AreaQuery>>(false, null, "Nie znaleziono żadnych obszarów");
        }

        // return areas
        _logger.LogInformation("Areas returned");
        return new ServiceResponse<IEnumerable<AreaQuery>>(true, areas, "Obszary zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<AreaQuery>>> GetAreasWithSpaces()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get areas
        var areas = await _context.Areas
            .AsNoTracking()
            .Select(a => new AreaQuery
            {
                AreaId = a.AreaId,
                Name = a.Name,
                Description = a.Description,
                IsDeleted = a.IsDeleted,
                UpdatedBy = a.UpdatedBy,
                CreatedBy = a.CreatedBy,
                CreatedOn = a.CreatedOn,
                UpdatedOn = a.UpdatedOn,
                Spaces = a.Spaces.Select(s => new SpaceQuery
                {
                    SpaceId = s.SpaceId,
                    Name = s.Name,
                    Description = s.Description,
                    IsDeleted = s.IsDeleted,
                    UpdatedBy = s.UpdatedBy,
                    CreatedBy = s.CreatedBy,
                    CreatedOn = s.CreatedOn,
                    UpdatedOn = s.UpdatedOn
                }).ToList()
            }).ToListAsync();
        if (areas is null)
        {
            _logger.LogWarning("No areas found");
            return new ServiceResponse<IEnumerable<AreaQuery>>(false, null, "Nie znaleziono żadnych obszarów");
        }
        // return areas
        _logger.LogInformation("Areas returned");
        return new ServiceResponse<IEnumerable<AreaQuery>>(true, areas, "Obszary zostały zwrócone");
    }

    public async Task<ServiceResponse<CoordinateQuery>> GetCoordinateByIdWithAssets(int coordinateId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateQuery
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                Assets = c.Assets.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn
                }).ToList()
            }).FirstOrDefaultAsync(c => c.CoordinateId == coordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse<CoordinateQuery>(false, null, "Nie znaleziono koordynatu");
        }
        // return coordinate
        _logger.LogInformation("Coordinate with id {CoordinateId} returned", coordinate.CoordinateId);
        return new ServiceResponse<CoordinateQuery>(true, coordinate, "Koordynat został zwrócony");
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateQuery>>> GetCoordinates()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinates
        var coordinates = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateQuery
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn
            }).ToListAsync();
        if (coordinates is null)
        {
            _logger.LogWarning("No coordinates found");
            return new ServiceResponse<IEnumerable<CoordinateQuery>>(false, null, "Nie znaleziono żadnych koordynatów");
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return new ServiceResponse<IEnumerable<CoordinateQuery>>(true, coordinates, "Koordynaty zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<CoordinateQuery>>> GetCoordinatesWithAssets()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinates
        var coordinates = await _context.Coordinates
            .AsNoTracking()
            .Select(c => new CoordinateQuery
            {
                CoordinateId = c.CoordinateId,
                Name = c.Name,
                Description = c.Description,
                IsDeleted = c.IsDeleted,
                UpdatedBy = c.UpdatedBy,
                CreatedBy = c.CreatedBy,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                Assets = c.Assets.Select(a => new AssetQuery
                {
                    AssetId = a.AssetId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn
                }).ToList()
            }).ToListAsync();
        if (coordinates is null)
        {
            _logger.LogWarning("No coordinates found");
            return new ServiceResponse<IEnumerable<CoordinateQuery>>(false, null, "Nie znaleziono żadnych koordynatów");
        }
        // return coordinates
        _logger.LogInformation("Coordinates returned");
        return new ServiceResponse<IEnumerable<CoordinateQuery>>(true, coordinates, "Koordynaty zostały zwrócone");
    }

    public async Task<ServiceResponse<PlantQuery>> GetPlantById(int plantId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plant
        var plant = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantQuery
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UpdatedBy = p.UpdatedBy,
                CreatedBy = p.CreatedBy,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn,
            }).FirstOrDefaultAsync(p => p.PlantId == plantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            return new ServiceResponse<PlantQuery>(false, null, "Nie znaleziono fabryki");
        }
        // return plant
        _logger.LogInformation("Plant with id {PlantId} returned", plant.PlantId);
        return new ServiceResponse<PlantQuery>(true, plant, "Fabryka została zwrócona");
    }

    public async Task<ServiceResponse<IEnumerable<PlantQuery>>> GetPlants()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plants
        var plants = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantQuery
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UpdatedBy = p.UpdatedBy,
                CreatedBy = p.CreatedBy,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn
            }).ToListAsync();
        if (plants is null)
        {
            _logger.LogWarning("No plants found");
            return new ServiceResponse<IEnumerable<PlantQuery>>(false, null, "Nie znaleziono żadnych fabryk");
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return new ServiceResponse<IEnumerable<PlantQuery>>(true, plants, "Fabryki zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<PlantQuery>>> GetPlantsWithAreas()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plants
        var plants = await _context.Plants
            .AsNoTracking()
            .Select(p => new PlantQuery
            {
                PlantId = p.PlantId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                UpdatedBy = p.UpdatedBy,
                CreatedBy = p.CreatedBy,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn,
                Areas = p.Areas.Select(a => new AreaQuery
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Description = a.Description,
                    IsDeleted = a.IsDeleted,
                    UpdatedBy = a.UpdatedBy,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn
                }).ToList()
            }).ToListAsync();
        if (plants is null)
        {
            _logger.LogWarning("No plants found");
            return new ServiceResponse<IEnumerable<PlantQuery>>(false, null, "Nie znaleziono żadnych fabryk");
        }
        // return plants
        _logger.LogInformation("Plants returned");
        return new ServiceResponse<IEnumerable<PlantQuery>>(true, plants, "Fabryki zostały zwrócone");
    }

    public async Task<ServiceResponse<SpaceQuery>> GetSpaceById(int spaceId)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get space
        var space = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceQuery
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                AreaId = s.AreaId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                UpdatedOn = s.UpdatedOn
            }).FirstOrDefaultAsync(s => s.SpaceId == spaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse<SpaceQuery>(false, null, "Nie znaleziono strefy");
        }
        // return space
        _logger.LogInformation("Space with id {SpaceId} returned", space.SpaceId);
        return new ServiceResponse<SpaceQuery>(true, space, "Strefa została zwrócona");
    }

    public async Task<ServiceResponse<IEnumerable<SpaceQuery>>> GetSpaces()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get spaces
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceQuery
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                AreaId = s.AreaId,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                UpdatedOn = s.UpdatedOn
            }).ToListAsync();
        if (spaces is null)
        {
            _logger.LogWarning("No spaces found");
            return new ServiceResponse<IEnumerable<SpaceQuery>>(false, null, "Nie znaleziono żadnych stref");
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return new ServiceResponse<IEnumerable<SpaceQuery>>(true, spaces, "Strefy zostały zwrócone");
    }

    public async Task<ServiceResponse<IEnumerable<SpaceQuery>>> GetSpacesWithCoordinates()
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get spaces
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Select(s => new SpaceQuery
            {
                SpaceId = s.SpaceId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UpdatedBy = s.UpdatedBy,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
                UpdatedOn = s.UpdatedOn,
                AreaId = s.AreaId,
                SpaceType = s.SpaceType,
                Coordinates = s.Coordinates.Select(c => new CoordinateQuery
                {
                    CoordinateId = c.CoordinateId,
                    Name = c.Name,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    UpdatedBy = c.UpdatedBy,
                    CreatedBy = c.CreatedBy,
                    CreatedOn = c.CreatedOn,
                    UpdatedOn = c.UpdatedOn,
                    SpaceId = c.SpaceId
                }).ToList()
            }).ToListAsync();
        if (spaces is null)
        {
            _logger.LogWarning("No spaces found");
            return new ServiceResponse<IEnumerable<SpaceQuery>>(false, null, "Nie znaleziono żadnych stref");
        }
        // return spaces
        _logger.LogInformation("Spaces returned");
        return new ServiceResponse<IEnumerable<SpaceQuery>>(true, spaces, "Strefy zostały zwrócone");
    }

    public async Task<ServiceResponse> MarkDeleteArea(AreaUpdateCommand areaUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var area = await _context.Areas
            .Include(a=>a.CommunicateAreas)
            .Include(a=>a.Spaces)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AreaId == areaUpdate.AreaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse(false, "Nie znaleziono obszaru");
        }
        if (area.IsDeleted)
        {
            _logger.LogWarning("Area already deleted");
            return new ServiceResponse(false, "Obszar już został oznaczony jako usunięty");
        }
        if (area.CommunicateAreas.Any(ca => !ca.IsDeleted))
        {
            _logger.LogWarning("Area has not deleted communicate areas");
            return new ServiceResponse(false, "Obszar posiada nie usunięte komunikaty dla strefy");
        }
        if (area.Spaces.Any(s => !s.IsDeleted))
        {
            _logger.LogWarning("Area has not deleted spaces");
            return new ServiceResponse(false, "Obszar posiada nie usunięte strefy");
        }
        
        area.IsDeleted = true;
        area.UpdatedBy = areaUpdate.UpdatedBy;

        _context.Areas.Update(area);
        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Strefa została oznaczona jako usunięta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking area as deleted");
            return new ServiceResponse(false, "Błąd podczas oznaczania strefy jako usuniętej");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCoordinate(CoordinateUpdateCommand coordinateUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await _context.Coordinates.Include(c => c.Assets)
            .Include(a=>a.CommunicateCoordinates)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateUpdate.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse(false, "Nie znaleziono koordynatu");
        }
        if (coordinate.IsDeleted)
        {
            _logger.LogWarning("Coordinate already marked as deleted");
            return new ServiceResponse(false, "Koordynat został już oznaczony jako usunięty");
        }
        // check if coordinate has active assets
        if (coordinate.Assets.Any(a => a.IsDeleted == false))
        {
            _logger.LogWarning("Coordinate has active assets");
            return new ServiceResponse(false, "Koordynat posiada aktywne zasoby");
        }
        if (coordinate.CommunicateCoordinates.Any(cc => !cc.IsDeleted))
        {
            _logger.LogWarning("Coordinate has active communicate coordinates");
            return new ServiceResponse(false, "Koordynat posiada aktywne komunikaty dla koordynatu");
        }

        // mark coordinate as deleted
        coordinate.IsDeleted = true;
        coordinate.UpdatedBy = coordinateUpdate.UpdatedBy;
        //update coordinate
        _context.Coordinates.Update(coordinate);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} marked as deleted", coordinate.CoordinateId);
            return new ServiceResponse(true, "Koordynat został oznaczony jako usunięty");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking coordinate as deleted");
            return new ServiceResponse(false, "Błąd podczas oznaczania koordynatu jako usuniętego");
        }
    }

    public async Task<ServiceResponse> MarkDeletePlant(PlantUpdateCommand plantUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var plant = await _context.Plants
            .Include(p => p.Areas)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PlantId == plantUpdate.PlantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            return new ServiceResponse(false, "Nie znaleziono fabryki");
        }
        if (plant.IsDeleted)
        {
            _logger.LogWarning("Plant already marked as deleted");
            return new ServiceResponse(false, "Fabryka została już oznaczona jako usunięta");
        }
        if (plant.Areas.Any(a => !a.IsDeleted))
        {
            _logger.LogWarning("Plant has not deleted areas");
            return new ServiceResponse(false, "Fabryka posiada nie usunięte obszary");
        }
        plant.IsDeleted = true;
        plant.UpdatedBy = plantUpdate.UpdatedBy;
        _context.Plants.Update(plant);
        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Fabryka została oznaczona jako usunięta");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking plant as deleted");
            return new ServiceResponse(false, "Błąd podczas oznaczania fabryki jako usuniętej");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSpace(SpaceUpdateCommand spaceUpdate)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var space = await _context.Spaces
            .Include(s => s.Coordinates)
            .Include(s => s.CommunicateSpaces)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SpaceId == spaceUpdate.SpaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse(false, "Nie znaleziono strefy");
        }
        if (space.IsDeleted)
        {
            _logger.LogWarning("Space already marked as deleted");
            return new ServiceResponse(false, "Strefa została już oznaczona jako usunięta");
        }
        if (space.Coordinates.Any(c => !c.IsDeleted))
        {
            _logger.LogWarning("Space has not deleted coordinates");
            return new ServiceResponse(false, "Strefa posiada nie usunięte koordynaty");
        }
        if (space.CommunicateSpaces.Any(cs => !cs.IsDeleted))
        {
            _logger.LogWarning("Space has not deleted communicate spaces");
            return new ServiceResponse(false, "Strefa posiada nie usunięte komunikaty dla strefy");
        }
        space.IsDeleted = true;
        space.UpdatedBy = spaceUpdate.UpdatedBy;
        _context.Spaces.Update(space);
        try
        {
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Strefa została oznaczona jako usunięta");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error marking space as deleted");
            return new ServiceResponse(false, "Błąd podczas oznaczania strefy jako usuniętej");
        }
    }
    public async Task<ServiceResponse> UpdateArea(AreaUpdateCommand areaUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get area
        var area = await _context.Areas
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AreaId == areaUpdateDto.AreaId);
        if (area == null)
        {
            _logger.LogWarning("Area not found");
            return new ServiceResponse(false, "Nie znaleziono obszaru");
        }
        // check for duplicate name
        var duplicate = await _context.Areas
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AreaId != areaUpdateDto.AreaId && a.PlantId == area.PlantId && a.Name.ToLower().Trim() == areaUpdateDto.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Duplicate area name");
            return new ServiceResponse(false, "Podana nazwa obszaru już istnieje w fabryce");
        }
        // update area
        area.Name = areaUpdateDto.Name;
        area.Description = areaUpdateDto.Description;
        area.IsDeleted = false;
        area.UpdatedBy = areaUpdateDto.UpdatedBy;
        // update area
        _context.Areas.Update(area);
        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Area with id {AreaId} updated", area.AreaId);
            return new ServiceResponse(true, "Obszar został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating area");

            return new ServiceResponse(false, "Błąd podczas aktualizacji obszaru");
        }
    }

    public async Task<ServiceResponse> UpdateCoordinate(CoordinateUpdateCommand coordinateUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get coordinate
        var coordinate = await _context.Coordinates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CoordinateId == coordinateUpdateDto.CoordinateId);
        if (coordinate == null)
        {
            _logger.LogWarning("Coordinate not found");
            return new ServiceResponse(false, "Nie znaleziono koordynatu");
        }
        // check for duplicate name
        var duplicate = await _context.Coordinates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CoordinateId != coordinateUpdateDto.CoordinateId && c.SpaceId == coordinate.SpaceId && c.Name.ToLower().Trim() == coordinateUpdateDto.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Duplicate coordinate name");
            return new ServiceResponse(false, "Podana nazwa koordynatu już istnieje w strefie");
        }
        // update coordinate
        coordinate.Name = coordinateUpdateDto.Name;
        coordinate.Description = coordinateUpdateDto.Description;
        coordinate.IsDeleted = false;
        coordinate.UpdatedBy = coordinateUpdateDto.UpdatedBy;
        // update coordinate
        _context.Coordinates.Update(coordinate);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Coordinate with id {CoordinateId} updated", coordinate.CoordinateId);
            return new ServiceResponse(true, "Koordynat został zaktualizowany");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating coordinate");

            return new ServiceResponse(false, "Błąd podczas aktualizacji koordynatu");
        }
    }

    public async Task<ServiceResponse> UpdatePlant(PlantUpdateCommand plantUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get plant
        var plant = await _context.Plants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PlantId == plantUpdateDto.PlantId);
        if (plant == null)
        {
            _logger.LogWarning("Plant not found");
            return new ServiceResponse(false, "Nie znaleziono fabryki");
        }
        // check for duplicate name
        var duplicate = await _context.Plants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PlantId != plantUpdateDto.PlantId && p.Name.ToLower().Trim() == plantUpdateDto.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Duplicate plant name");
            return new ServiceResponse(false, "Podana nazwa fabryki już istnieje");
        }

        // update plant
        plant.Name = plantUpdateDto.Name;
        plant.Description = plantUpdateDto.Description;
        plant.IsDeleted = false;
        plant.UpdatedBy = plantUpdateDto.UpdatedBy;
        // update plant
        _context.Plants.Update(plant);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Plant with id {PlantId} updated", plant.PlantId);
            return new ServiceResponse(true, "Fabryka została zaktualizowana");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating plant");

            return new ServiceResponse(false, "Błąd podczas aktualizacji fabryki");
        }
    }

    public async Task<ServiceResponse> UpdateSpace(SpaceUpdateCommand spaceUpdateDto)
    {
        await using var _context = await _factory.CreateDbContextAsync();

        // get space
        var space = await _context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SpaceId == spaceUpdateDto.SpaceId);
        if (space == null)
        {
            _logger.LogWarning("Space not found");
            return new ServiceResponse(false, "Nie znaleziono strefy");
        }
        // check for duplicate name
        var duplicate = await _context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SpaceId != spaceUpdateDto.SpaceId && s.AreaId == space.AreaId && s.Name.ToLower().Trim() == spaceUpdateDto.Name.ToLower().Trim());
        if (duplicate != null)
        {
            _logger.LogWarning("Duplicate space name");
            return new ServiceResponse(false, "Podana nazwa strefy już istnieje w obszarze");
        }
        // update space
        space.Name = spaceUpdateDto.Name;
        space.Description = spaceUpdateDto.Description;
        space.IsDeleted = false;
        space.UpdatedBy = spaceUpdateDto.UpdatedBy;
        // update space
        _context.Spaces.Update(space);

        try
        {
            // save changes
            await _context.SaveChangesAsync();

            // return success
            _logger.LogInformation("Space with id {SpaceId} updated", space.SpaceId);
            return new ServiceResponse(true, "Strefa została zaktualizowana");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating space");

            return new ServiceResponse(false, "Błąd podczas aktualizacji strefy");
        }
    }
}