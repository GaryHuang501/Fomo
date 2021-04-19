using FomoAPI.Application.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FomoAPI.Application.ViewModels
{
    /// <summary>
    /// UI Data to display on the members page.
    /// </summary>
    public class MembersViewModel
    {   
        public IDictionary<char, List<MemberDTO>> MemberGroupings { get; private set; }

        public IEnumerable<MemberDTO> UncategorizedMembers { get; private set; }

        public int Total { get; private set; }

        public int Offset { get; private set; }

        public int Limit { get; private set; }


        public MembersViewModel(IEnumerable<MemberDTO> members, int total, int offset, int limit)
        {
            Total = total;
            Offset = offset;
            Limit = limit;

            var uncategorizedMembers = new List<MemberDTO>();

            MemberGroupings = CreateMemberGrouping();

            foreach(var member in members)
            {
                char firstLetter = char.ToUpper(member.Name[0]);

                if (MemberGroupings.ContainsKey(firstLetter))
                {
                    MemberGroupings[firstLetter].Add(member);
                }
                else
                {
                    uncategorizedMembers.Add(member);
                }
            }

            UncategorizedMembers = uncategorizedMembers;
        }

        [JsonConstructor]
        public MembersViewModel(IDictionary<char, List<MemberDTO>> memberGroupings, IEnumerable<MemberDTO> uncategorizedMembers, int total, int offset, int limit)
        {
            MemberGroupings = memberGroupings;
            UncategorizedMembers = uncategorizedMembers;
            Total = total;
            Offset = offset;
            Limit = limit;
        }

        private Dictionary<char, List<MemberDTO>> CreateMemberGrouping()
        {
            var membersByFirstChar = new Dictionary<char, List<MemberDTO>>();

            for(char letter = 'A'; letter <= 'Z'; letter++)
            {
                membersByFirstChar.Add(letter, new List<MemberDTO>());                  
            }

            return membersByFirstChar;
        }
    }
}
