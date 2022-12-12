//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaManagementProject.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Film
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Film()
        {
            this.ShowTimes = new HashSet<ShowTime>();
        }
    
        public int Id { get; set; }
        public string FilmName { get; set; }
        public Nullable<int> Duration { get; set; }
        public string FilmType { get; set; }
        public string Country { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public string FilmInfo { get; set; }
        public bool IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowTime> ShowTimes { get; set; }
    }
}
