﻿using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupGeneratorService
    {
        Task<List<LineupsModel>> GetAsync(LineupRequestModel request);
    }
}
