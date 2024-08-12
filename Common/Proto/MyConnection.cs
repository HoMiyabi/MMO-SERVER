using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Summer
{
    public class MyConnection : Connection
    {
        public MyConnection(Socket socket) : base(socket)
        {
        }


        private Proto.Package _package = null;

        public Proto.Request Request
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Request == null)
                {
                    _package.Request = new();
                }
                return _package.Request;
            }
        }

        public Proto.Response Response
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Response == null)
                {
                    _package.Response = new();
                }
                return _package.Response;
            }
        }

        public void Send()
        {
            if (_package != null)
            {
                Send(_package);
                _package = null;
            }
        }
    }
}
