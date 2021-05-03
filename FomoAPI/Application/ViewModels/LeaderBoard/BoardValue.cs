using FomoAPI.Application.DTOs;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Application.ViewModels
{
    public class BoardValue
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Value { get; private set; }

        [JsonConstructor]
        public BoardValue(string id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }
}
