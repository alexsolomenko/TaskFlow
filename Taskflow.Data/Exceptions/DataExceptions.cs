using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskflow.Data.Exceptions;

public class UserAlreadyExistsException: Exception
{
    public UserAlreadyExistsException() : base(Localization.errors.UserAlreadyExists) { }
}
