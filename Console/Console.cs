using System.Collections.Generic;
using Wayfarer.Utils.Debug;

namespace Wayfarer.Console
{
    public class Console
    {
        private readonly Parser _parser = new Parser();

        public void Init(ConsoleCommandBase cmdBase)
        {
            _parser.SetConsole(this);
            _parser.SetCommandBase(cmdBase);
        }

        public void Exec(string entry)
        {
            _parser.Parse(entry);
        }

        public void Echo(string entry)
        {
            Log.Console(entry);
        }
    }
}