using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.Exceptions;

public class WorkspaceNotFoundException : NotFoundException
{
    public WorkspaceNotFoundException(string message, int id) : base(message, id)
    {
        
    }
}
