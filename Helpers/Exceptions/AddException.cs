// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Econolite.Ode.Helpers.Exceptions
{
    public sealed class AddException : Exception
    {
        public AddException(string message): base(message)
        {
        }
    }
}
