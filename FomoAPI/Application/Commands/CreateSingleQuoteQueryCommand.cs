using System.Runtime.Serialization;

namespace FomoAPI.Application.Commands
{
    public class CreateSingleQuoteQueryCommand
    {
        [DataMember(IsRequired = true)]
        public string Symbol { get; set; }

        public CreateSingleQuoteQueryCommand(string symbol)
        {
            Symbol = symbol;
        }
    }
}
