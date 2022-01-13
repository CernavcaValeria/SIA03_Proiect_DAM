using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect.Class
{
    public class Leader
    {
        public int LeaderId;

        public List<Team> ListOfTeams
        {
            get;
            set;
        } = new List<Team>();

        public Leader(int leaderId, List<Team> listOfTeams)
        {
            LeaderId = leaderId;
            ListOfTeams = listOfTeams;
        }

        public Leader(int leaderId)
        {
            LeaderId = leaderId;
        }

        public Leader() { }

        public override bool Equals(object obj)
        {
            return obj is Leader leader &&
                   LeaderId == leader.LeaderId &&
                   EqualityComparer<List<Team>>.Default.Equals(ListOfTeams, leader.ListOfTeams);
        }

        public override int GetHashCode()
        {
            int hashCode = 412810611;
            hashCode = hashCode * -1521134295 + LeaderId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Team>>.Default.GetHashCode(ListOfTeams);
            return hashCode;
        }
    }
}
