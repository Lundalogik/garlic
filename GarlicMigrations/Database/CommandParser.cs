using System;
using System.Collections.Generic;

namespace Garlic.Database
{
    public static class CommandParser
    {
        public static IEnumerable<string> NewLineWith(string keyword, string batch)
        {
            var commands = new List<string>();
            string command = "";
            keyword = keyword.ToLower();

            string[] lines = batch.Split('\n');
            foreach (var l in lines)
            {
                if (l.Trim().ToLower() != keyword)
                {
                    command += l + '\n';
                }
                else
                {
                    if (!String.IsNullOrEmpty(command))
                    {
                        commands.Add(command);
                        command = "";
                    }
                }
            }

            if (!String.IsNullOrEmpty(command))
            {
                commands.Add(command);
            }

            return commands;            
        }

        public static IEnumerable<string> EndsWith(string keyword, string batch)
        {
            var commands = new List<string>();
            string command = "";
            keyword = keyword.ToLower();

            string[] lines = batch.Split('\n');
            foreach (var l in lines)
            {
                if (l.Trim().ToLower().EndsWith(keyword))
                {
                    command += l;
                    commands.Add(command);
                    command = "";
                }
                else
                {
                    command += l + '\n';                    
                }                  
            }

            if (!String.IsNullOrEmpty(command))
            {
                commands.Add(command);
            }

            return commands;            
            
        }
    }
}