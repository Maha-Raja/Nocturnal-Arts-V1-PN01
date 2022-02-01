using Nat.Core.Lookup.Model;
using System;

namespace Nat.EventApp.Services.ViewModels
{
	public class ArtistLovViewModel 
	{
		public Int32 Id { get; set; }
        public Nullable<Int32> PlannerId { get; set; }
        public String Value { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Double Rating { get; set; }
        public String Image { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }

        public static implicit operator string(ArtistLovViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}
