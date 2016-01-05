using cn.bmob.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.bmob.api.unit
{
    public class GameObject : BmobTable
    {
        public override string table
        {
            get
            {
                if (fTable != null)
                {
                    return fTable;
                }
                return base.table;
            }
        }

        private String fTable;
        public GameObject() { }

        public GameObject(string table)
        {
            this.fTable = table;
        }

        public List<int> arrint { get; set; }
        public List<string> arrstring { get; set; }
        public BmobInt jo { get; set; }
        public BmobInt jo2 { get; set; }
        public BmobInt obj { get; set; }
        public string s { get; set; }

        public BmobPointer<BmobUser> user { get; set; }

        public override void readFields(BmobInput input)
        {
            base.readFields(input);

            this.arrint = input.getList<int>("arrint");
            this.arrstring = input.getList<string>("arrstring");
            this.jo = input.getInt("jo");
            this.jo2 = input.getInt("jo2");
            this.obj = input.getInt("obj");
            this.s = input.getString("s");


            this.user  = input.Get<BmobPointer<BmobUser>>("user");
        }

        public override void write(BmobOutput output, Boolean all)
        {
            base.write(output, all);

            output.Put("arrint", this.arrint);
            output.Put("arrstring", this.arrstring);

            output.Put("jo", this.jo);
            output.Put("jo2", this.jo2);
            output.Put("obj", this.obj);
            output.Put("s", this.s);

            output.Put("user", this.user);
        }
    }
}
