using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HungProjects.Models;

namespace HungProjects.Controllers.TrangChu
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu

        QLKhachSanDataContext data = new QLKhachSanDataContext();
        public ActionResult Index(string loi)
        {
            switch(loi)
            {
                case "KhongCoTrongDatabase":
                    {
                        ViewBag.ThongBao = "Database vẫn chưa có data";
                        break;
                    }
                case "TimThayKhachSan":
                    {
                        ViewBag.ThongBao = "Danh sách các khách hàng";
                        break;
                    }
                case "KhongTimThayKS":
                    {
                        ViewBag.ThongBao = "Không tìm thấy khách sạn nào.";
                        break;
                    }
                default :
                    {
                        ViewBag.ThongBao = "không có từ khóa để tìm.";
                        break;
                    }
            }
            return View();
        }

        public ActionResult TimKiemKhachSan()
        {

            return PartialView();
        }

        //tìm kiếm danh sách khách sạn theo tên khách sạn hoặc địa chỉ nào đó
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult TimKhachSan(FormCollection frmCollection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string chuoiTimKiem = frmCollection["txtTimKiem"].ToString();
                    int madiachi = data.DiaDiems.Where(n => n.TenDiaDiem == chuoiTimKiem).FirstOrDefault().MaDiaDiem;
                    List<KhachSan> lstKhachSan = data.KhachSans.Where(n => n.TenKhachSan.Contains(chuoiTimKiem) || n.MaDiaDiem == madiachi).OrderByDescending(n => n.TenKhachSan).ToList();
                    //Phan trang
                    

                    ViewBag.chuoiTimKiem = chuoiTimKiem;
                    //neu ket qua ko tim thay hang
                    if (lstKhachSan.Count == 0)
                    {
                        ViewBag.ThongBao = "Không tìm thấy Khách sạn nào.";
                    }
                    return View(lstKhachSan.OrderBy(n => n.TenKhachSan).ToList());
                }
                return RedirectToAction("TimKhachSan", "TrangChu", new {loi = "TimThayKhachSan" });
            }
            catch (Exception error)
            { 
                return RedirectToAction("Index", "TrangChu", new { loi = "KhongCoTrongDatabase" });
            } 
        }


        [HttpGet, ValidateInput(false)]
        public ActionResult TimKhachSan(string chuoiTimKiem = " ")
        {
            try
            {
                ViewBag.chuoiTimKiem = chuoiTimKiem;

                int madiachi = data.DiaDiems.Where(n => n.TenDiaDiem == chuoiTimKiem).FirstOrDefault().MaDiaDiem;

                List<KhachSan> lstKhachSan = data.KhachSans.Where(n => n.TenKhachSan.Contains(chuoiTimKiem) || n.MaDiaDiem == madiachi).ToList();
                  
                if (lstKhachSan.Count == 0)
                {
                    ViewBag.ThongBao = "Không tìm thấy Khách sạn nào.";
                }

                ViewBag.ThongBao = chuoiTimKiem;
                return View(lstKhachSan.OrderBy(n => n.TenKhachSan).ToList());
            }
            catch (Exception error)
            { 
                return RedirectToAction("Index", "TrangChu", new { loi = "KhongCoTrongDatabase" });
            }
        }

        public ActionResult danhSachDiaChi()
        {
            List<DiaDiem> dsDD = data.DiaDiems.ToList();
            return PartialView(dsDD);
        }
         
        public ActionResult KhachSanTheoDiaDiem(FormCollection frmKSTheoDD)
        {
            try
            {
                var diachi = frmKSTheoDD["madiadiem"];
                int IDDiaChi = int.Parse(diachi.ToString());
                List<KhachSan> lstKhachSan = data.KhachSans.Where(n => n.MaDiaDiem == IDDiaChi).ToList();
                if (lstKhachSan.Count == 0 || lstKhachSan == null)
                {
                    return RedirectToAction("Index", "TrangChu", new { loi = "KhongTimThayKS" });
                }
                else
                {
                    return View(lstKhachSan);
                }
                return View(lstKhachSan);
            }
            catch (Exception error)
            {
                return RedirectToAction("Index", "TrangChu", new { loi = "default" });
            }
            
        }
        public ActionResult ChitietKhachsan (int id)
        {
            KhachSan ksan = data.KhachSans.SingleOrDefault(m => m.MaKhachSan == id);
            return View(ksan);
        }
        public ActionResult Chinhsach(int id)
        {
            //var chinhsach = from chs in data.ChinhSaches
            //                select chs;
            ChinhSach chinhsach = data.ChinhSaches.SingleOrDefault(m => m.MaKhachSan == id);
            return PartialView(chinhsach);
        }

        public ActionResult DanhsachPhongKS(int id)
        { 
            List<ChiTiet_LoaiPhong> ctLoaiPhong = data.ChiTiet_LoaiPhongs.Where(m => m.MaKhachSan == id).ToList();
            return PartialView(ctLoaiPhong);
        }
    }
}