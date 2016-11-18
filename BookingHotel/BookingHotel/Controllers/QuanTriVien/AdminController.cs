using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingHotel.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace BookingHotel.Controllers.QuanTriVien
{
    public class AdminController : Controller
    {
        DataBookingHotelDataContext db = new DataBookingHotelDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["error0"] = "chưa nhập: tên đăng nhập";
            }
            if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["error1"] = "chưa nhập: mật khẩu";
            }
            else
            {
                QuanTri qt = db.QuanTris.SingleOrDefault(n => n.Username_QT == tendn && n.Password_QT == matkhau);
                if (qt != null)
                {
                    // viewbag.Thong bao = "Đăng nhập thành công"
                    Session["Taikhoanadmin"] = qt;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult Khachsan(int? page)
        {
            //tạo biến quy định cho số sản phẩm trên mỗi trang

            //tạo biến số trang
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(data.SANPHAMs.ToList());
            return View(db.KhachSans.ToList().OrderBy(n => n.MaKhachSan).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]        
        public ActionResult ThemKhachsan()
        {
            // đưa dữ liệu vào dropdown List
            //lấy dữ liệu từ tab chủ đề, sắp xếp tăng dần theo tên chủ đề, chọn lấy giá trị THUONGHIEU, NOISANXUAT
            ViewBag.MaDiaDiem = new SelectList(db.DiaDiems.ToList().OrderBy(n => n.TenDiaDiem), "MaDiaDiem", "TenDiaDiem");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemKhachsan(KhachSan ksan, HttpPostedFileBase fileUpload)
        {
            // đưa dữ liệu vào Dropdownload
            ViewBag.MaDiaDiem = new SelectList(db.DiaDiems.ToList().OrderBy(n => n.TenDiaDiem), "MaDiaDiem", "TenDiaDiem");
            //kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            // Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, lưu ý bổ sung thư viện using System.IO
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Hinhanh/Khachsan/"), fileName);
                    // kiểm tra hình ảnh tồn tại chưa
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //lưu hình ảnh vào đường dẫn.
                        fileUpload.SaveAs(path);
                    }
                    ksan.AnhBia = fileName;
                    //lưu vào CSDL
                    db.KhachSans.InsertOnSubmit(ksan);
                    db.SubmitChanges();
                }
                return RedirectToAction("Khachsan");
            }
        }
        public ActionResult ChitietKhachsan(int id)
        {
            //lấy ra đối tượng sách theo mã
            KhachSan ksan = db.KhachSans.SingleOrDefault(model => model.MaKhachSan == id);
            ViewBag.MaKhachSan = ksan.MaKhachSan;
            if (ksan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ksan);
        }
        [HttpGet]
        public ActionResult XoaKhachsan(int id)
        {
            KhachSan ksan = db.KhachSans.SingleOrDefault(model => model.MaKhachSan == id);
            ViewBag.MaKhachSan = ksan.MaKhachSan;
            if (ksan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ksan);
        }
        [HttpPost, ActionName("XoaKhachsan")]
        public ActionResult Xacnhanxoa(int id)
        {
            // lấy ra đối tượng sách cần xoa theo mã
            KhachSan ksan = db.KhachSans.SingleOrDefault(model => model.MaKhachSan == id);
            ViewBag.MaKhachSan = ksan.MaKhachSan;
            if (ksan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KhachSans.DeleteOnSubmit(ksan);
            db.SubmitChanges();
            return RedirectToAction("Khachsan");
        }
        [HttpGet]
        public ActionResult SuaKhachsan(int id)
        {
            // lấy ra đối tượng sách cần xoa theo mã
            KhachSan ksan = db.KhachSans.SingleOrDefault(model => model.MaKhachSan == id);
            ViewBag.MaKhachSan = ksan.MaKhachSan;
            if (ksan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaDiaDiem = new SelectList(db.DiaDiems.ToList().OrderBy(n => n.TenDiaDiem), "MaDiaDiem", "TenDiaDiem", ksan.MaDiaDiem);
            return View(ksan);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaKhachsan(KhachSan ksan, HttpPostedFileBase fileUpload, FormCollection collection, int id)
        {
            ViewBag.MaDiaDiem = new SelectList(db.DiaDiems.ToList().OrderBy(n => n.TenDiaDiem), "MaDiaDiem", "TenDiaDiem", ksan.MaDiaDiem);
            KhachSan ks = db.KhachSans.Where(m => m.MaKhachSan == ksan.MaKhachSan).FirstOrDefault();
            string tenkhachsan = collection["TenKhachSan"];
            var diachi = collection["DiaChi"];
            var thongtin = collection["ThongTin"];
            string video = collection["Video"];
            if (String.IsNullOrEmpty(tenkhachsan))
            {
                ViewData["Loi"] = "Tên ko để trống";
            }
            else
            {
                ks.TenKhachSan = tenkhachsan;
                ks.DiaChi = diachi;
                ks.ThongTin = thongtin;
                ks.Video = video;
                //Luu vao CSDL   
                UpdateModel(ksan);
                db.SubmitChanges();
                //kiểm tra đường dẫn file
                return RedirectToAction("Khachsan");
            }
            return this.SuaKhachsan(id);

        }
        [HttpGet]
        public ActionResult ThemChinhsach(KhachSan ksan)
        {
            ViewBag.MaKhachSan = new SelectList(db.KhachSans.ToList().OrderBy(n => n.TenKhachSan), "MaKhachSan", "TenKhachSan", ksan.MaKhachSan);
            return PartialView();
        }
    }
}