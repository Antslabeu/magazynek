using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magazynek.Entities
{
    public class Session {
        public Guid guid { get; private set; }
        public User? user { get; private set; }
        public DateTime modifiedAt { get; private set; }

        public Session(Guid guid) {
            this.guid = guid;
            this.modifiedAt = DateTime.Now;
        }
        public void UpdateUser(User? user)
        {
            this.user = user;
            this.modifiedAt = DateTime.Now;
        }
        public void RefreshSessionIfLogged()
        {
            if(user != null) this.modifiedAt = DateTime.Now;
        }
    }
}