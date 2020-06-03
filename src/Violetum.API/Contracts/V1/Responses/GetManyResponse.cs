﻿using System.Collections.Generic;

namespace Violetum.API.Contracts.V1.Responses
{
    public class GetManyResponse<TViewModel>
    {
        public IEnumerable<TViewModel> Data { get; set; }
        public int Count { get; set; }
        public Params Params { get; set; }
    }

    public class Params
    {
        public int Limit { get; set; }
        public int CurrentPage { get; set; }
    }
}