using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CenterSpace.NMath.Stats;

namespace Anova.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Generic app description for a page that will never be viewed";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Srdjan Hajder";

            return View();
        }

        [HttpPost]
        public ActionResult Calculator(int AltNo, int MeasureNo)
        {
            ViewBag.AltNo = AltNo;
            ViewBag.MeasureNo = MeasureNo;
            return View();
        }
        [HttpPost]
        public ActionResult Result(FormCollection form)
        {
            int k = form.AllKeys.Length, n = form.GetValues(0).Length;
            double[] averages = new double[k];
            double[] alts;
            double[][] measurements = new double[k][];
            double sum = 0, avg;
            for (int i = 0; i < form.Keys.Count; i++)
            {
                measurements[i] = form.GetValues(i).Select(x => double.Parse(x)).ToArray();
                averages[i] = measurements[i].Average();
                sum += measurements[i].Sum();
            }
            avg = sum / (k * n);
            alts = averages.Select(x => avg - x).ToArray();
            double ssa = 0, sse = 0;

            ssa = n * (averages.Select(x => Math.Pow((x - avg), 2)).Sum());
            for (int j = 0; j < k; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    sse += Math.Pow(measurements[j][i] - averages[j], 2);
                }
            }
            double Sa2 = ssa / (k - 1);
            double Se2 = (sse / (k * (n - 1)));
            ViewBag.SSE = sse;
            ViewBag.SSA = ssa;
            ViewBag.Ftest = Sa2 / Se2;
            ViewBag.Sc = Math.Pow(((2 *Se2) / (k * n)), 0.5);
            ViewBag.alts = alts;
            ViewBag.n = n;
            ViewBag.k = k;
            return View();
        }

        [HttpPost]
        public ActionResult Contrast(FormCollection form)
        {
            ViewBag.SSA = form.GetValues("ssa").Select(x => double.Parse(x)).FirstOrDefault();
            ViewBag.SSE = form.GetValues("sse").Select(x => double.Parse(x)).FirstOrDefault();
            ViewBag.Ftest = form.GetValues("ftest").Select(x => double.Parse(x)).FirstOrDefault();
            ViewBag.Sc = form.GetValues("Sc").Select(x => double.Parse(x)).FirstOrDefault();
            ViewBag.n = form.GetValues("n").Select(x => int.Parse(x)).FirstOrDefault();
            ViewBag.k = form.GetValues("k").Select(x => int.Parse(x)).FirstOrDefault();
            ViewBag.Alt1 = form.GetValues("alt1").Select(x => int.Parse(x)).FirstOrDefault();
            ViewBag.Alt2 = form.GetValues("alt2").Select(x => int.Parse(x)).FirstOrDefault();
            ViewBag.Certainty = form.GetValues("certainty").Select(x=>double.Parse(x)).FirstOrDefault();

            string alts = form.GetValues("alts").FirstOrDefault();
            ViewBag.alts = alts.Split(',').Select(x => double.Parse(x)).ToArray();

            double c = ViewBag.alts[ViewBag.Alt1-1] - ViewBag.alts[ViewBag.Alt2-1];
            var tdist = new TDistribution(ViewBag.k * (ViewBag.n - 1));
            double alpha = 1 - ViewBag.Certainty;
            double div = tdist.InverseCDF(1-alpha/2)*ViewBag.Sc;
            ViewBag.LowLimit = c - div;
            ViewBag.HighLimit = c + div;

            return View("Result");
        }
    }
}