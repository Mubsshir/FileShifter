using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShifter
{
    class MainProgram
    {
        public static void StartProgram()
        {
            FileShifter fileShifter = new FileShifter();
            fileShifter.StartFileMoveMent();
        }
  
    }
}
