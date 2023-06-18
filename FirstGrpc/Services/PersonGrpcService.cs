using Google.Protobuf.Collections;
using Grpc.Core;

namespace FirstGrpc.Services;

public class PersonGrpcService : FirstGrpc.PersonService.PersonServiceBase
{
    private readonly IPersonRepository _repository;
    private readonly ILogger<PersonGrpcService> _logger;
    public PersonGrpcService(IPersonRepository repository, ILogger<PersonGrpcService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override Task<GeneralResponse> AddPerson(CreatePersonRequest request, ServerCallContext context)
    {
        var result = _repository.AddPerson(
            new Person
            {
                Name = request.Name,
                Family = request.Family,
                BirthDay = new DateTime(Convert.ToInt64(request.BirthDay))
            });
        return Task.FromResult(new GeneralResponse
        {
            Message = result ? "done" : "error",
            IsSuccessful = result
        });
    }

    public override Task<GetAllResponse> GetAllPerson(GetAllRequest request, ServerCallContext context)
    {
        var items = _repository.GetAll(request.Query);
        var response =  new GetAllResponse
        {
            Message = "done",
            IsSuccessful = true,
            
        };
        foreach (var person in items)
        {
            response.Items.Add(new FirstGrpc.Person
            {
                Family = person.Family,
                Name = person.Name,
                BirthDay = Convert.ToUInt64(person.BirthDay.Ticks),
                Id = person.Id.ToString()
            });   
        }
        
        return Task.FromResult(response);
    }
}

public interface IPersonRepository
{
    bool AddPerson(Person person);
    IReadOnlyList<Person> GetAll(string query);
}

public class Person
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Family { get; set; } = "";
    public DateTime BirthDay { get; set; }
}

public class PersonInMemoryRepository: IPersonRepository
{
    private List<Person> _dataSource;

    public PersonInMemoryRepository()
    {
        _dataSource = new List<Person>();
    }

    public bool AddPerson(Person person)
    {
        if (_dataSource.All(x => x.Id != person.Id))
        {
            _dataSource.Add(person);
            return true;
        }
        return false;
    }

    public IReadOnlyList<Person> GetAll(string query)
    {
        if (string.IsNullOrEmpty(query)) return _dataSource.AsReadOnly();
        return _dataSource.Where(x => x.Name.Contains(query) || x.Family.Contains(query)).ToList().AsReadOnly();
    }
}