using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcing.SqlAdapter
{
    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
