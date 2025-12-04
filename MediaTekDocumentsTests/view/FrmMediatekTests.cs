using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.view.Tests
{
    [TestClass()]
    public class FrmMediatekTests
    {

        [TestMethod()]
        public void ParutionDansAbonnementTest()
        {
            FrmMediatek frm = new FrmMediatek();

            DateTime j = DateTime.Now;
            DateTime j5 = DateTime.Now + TimeSpan.FromDays(5);
            DateTime j10 = DateTime.Now + TimeSpan.FromDays(10);

            Assert.IsFalse(frm.ParutionDansAbonnement(j, j5, j10));
            Assert.IsTrue(frm.ParutionDansAbonnement(j5, j, j10));
        }
    }
}