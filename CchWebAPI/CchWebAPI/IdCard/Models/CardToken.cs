﻿using System;

//MUST REMAIN BACKWARD COMPATIBLE
namespace CchWebAPI.IdCard.Models {
    public class CardToken {
        public int EmployerId { get; set; }
        public DateTime Expires { get; set; }
        public CardDetail CardDetail { get; set; }
    }
}