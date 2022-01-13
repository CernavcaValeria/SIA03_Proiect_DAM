using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS_Proiect
{
    public class Team
    {
        public int TeamId;

        public List<User> TeamComponents
        {
            get;
            set;
        } = new List<User>();

        public Team(int teamId, List<User> teamComponents)
        {
            TeamId = teamId;
            TeamComponents = teamComponents;
        }

        public Team(int teamId)
        {
            TeamId = teamId;
        }

        public Team(List<User> teamComponents)
        {
            TeamComponents = teamComponents;
        }

        public Team() { }

        public override bool Equals(object obj)
        {
            return obj is Team team &&
                   TeamId == team.TeamId &&
                   EqualityComparer<List<User>>.Default.Equals(TeamComponents, team.TeamComponents);
        }

        public override int GetHashCode()
        {
            int hashCode = -48120239;
            hashCode = hashCode * -1521134295 + TeamId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<User>>.Default.GetHashCode(TeamComponents);
            return hashCode;
        }
    }
}
