﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TinTucMoiNhat.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class tintucmoinhatEntities : DbContext
    {
        public tintucmoinhatEntities()
            : base("name=tintucmoinhatEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<page> pages { get; set; }
        public virtual DbSet<channel> channels { get; set; }
        public virtual DbSet<video> videos { get; set; }
        public virtual DbSet<post> posts { get; set; }
        public virtual DbSet<location> locations { get; set; }
        public virtual DbSet<news_business> news_business { get; set; }
        public virtual DbSet<weather> weathers { get; set; }
        public virtual DbSet<weather_code> weather_code { get; set; }
        public virtual DbSet<notification> notifications { get; set; }
        public virtual DbSet<comment> comments { get; set; }
        public virtual DbSet<profile> profiles { get; set; }
        public virtual DbSet<news> news { get; set; }
    }
}
