namespace MiniProjet_.NET.Models
{
    public class DetailCommandeRevendeur
    {
        public int Id { get; set; }


        public decimal Prix { get; set; }
        public int Qte { get; set; }
        public decimal SousTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Many to Many [ Commande Revendeur - Variation ]
        public int VariationId { get; set; } // Required foreign key property
        public virtual Variation Variation { get; set; }  // Required reference navigation to principal
        public int RevendeurCommandeId { get; set; }
        public RevendeurCommande RevendeurCommande { get; set; }
        //
    }
}
