namespace Backend.Models
{
    //model voor het helpen met verwerken van de data voor het updaten van status van verhuuraanvragen 
    //door de backoffice medewerker in voertuigcontroller
    public class BackOfficeModel
    {
        public int AanvraagID { get; set; }
        public string Status { get; set; }
    }
}
