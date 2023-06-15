using System.Threading.Tasks;
using Byndyusoft.Example.Domain.Dtos;

namespace Byndyusoft.Example.Domain.Services.Interfaces
{
    public interface IExampleService
    {
        public Task DoSomething(ExampleMessageDto message);
    }
}