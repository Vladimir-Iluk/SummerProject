using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class ExceptionNotFound : Exception
    {
        public ExceptionNotFound(string entityName, Guid id) : base($"{entityName} not found by id {id}") { }
    }
}
