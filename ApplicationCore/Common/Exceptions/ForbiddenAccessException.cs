﻿namespace ApplicationCore.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base("No tienes permisos") {}
    }
}
