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
    
        public virtual DbSet<news> news { get; set; }
        public virtual DbSet<page> pages { get; set; }
        public virtual DbSet<channel> channels { get; set; }
        public virtual DbSet<video> videos { get; set; }
        public virtual DbSet<post> posts { get; set; }
    }
}
