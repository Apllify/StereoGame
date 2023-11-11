using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StereoGame.Extensions;
using UnfinishedBusinessman.StereoGame.Extensions;

namespace StereoGame.Particles
{
    /// <summary>
    /// A data record which encapsulates a range of possible
    /// particles. Used as an input to a particle emitter.
    /// </summary>
    public record class ParticleTemplate
	{
		public float MinVelocity = 0;
		public float MaxVelocity = 0;
		public float Acceleration = 0;

		public ColorF StartPColor = Color.White.ToColorF();
		public ColorF? EndPColor = null;

		public float MinLifeSpan = 0.3f;
		public float MaxLifeSpan = 1;

		public float Size = 5;

		public float GrowthRate = 0;
	};
}
