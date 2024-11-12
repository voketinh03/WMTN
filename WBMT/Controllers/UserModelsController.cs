using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using WBMT.Data;
using WBMT.Models;

namespace WBMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserModelsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
            
        public UserModelsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("dangnhap")]
        public JsonResult DN([FromBody] UserModel userModel)
        {
            try
            {
                // Sửa lại câu truy vấn SQL cho đúng
                string query = @"
            SELECT u.username, u.password, r.id AS role_id, r.name
            FROM Users u
            JOIN Roles r ON u.role_id = r.id
            WHERE u.username = @username AND u.password = @password";

                string sqlDataSource = _configuration.GetConnectionString("WBMT");
                string role = string.Empty;
                string username = string.Empty;
                string roleName = string.Empty;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", userModel.UserName);
                        myCommand.Parameters.AddWithValue("@password", userModel.Password);

                        using (SqlDataReader reader = myCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                username = reader["username"].ToString();
                                roleName = reader["name"].ToString();
                            }
                        }
                    }
                }

                // Kiểm tra xem người dùng có hợp lệ và có quyền gì
                if (!string.IsNullOrEmpty(username))
                {
                    // Điều hướng dựa trên role
                    string redirectUrl = roleName.ToLower() == "admin" ? "admin" : "ketinh";

                    return new JsonResult(new
                    {
                        status = "success",
                        message = "Login successful",
                        data = new { username, roleName, redirectUrl }
                    });
                }
                else
                {
                    return new JsonResult(new { status = "failure", message = "Invalid username or password" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { status = "error", message = $"An error occurred: {ex.Message}" });
            }
        }

        //
        //DK
        //[HttpPost("dangky")]
        //public JsonResult Register([FromBody] UserModel userModel)
        //{
        //    try
        //    {
        //        // Kiểm tra nếu mật khẩu và xác nhận mật khẩu không khớp
        //        if (userModel.Password != userModel.ConfirmPassword)
        //        {
        //            return new JsonResult(new { status = "failure", message = "Passwords do not match" });
        //        }

        //        // Kiểm tra nếu tên người dùng đã tồn tại
        //        string queryCheckUser = "SELECT COUNT(*) AS UserCount FROM Users WHERE username = @username";
        //        int userCount = 0;
        //        string sqlDataSource = _configuration.GetConnectionString("WBMT");

        //        using (SqlConnection myCon = new SqlConnection(sqlDataSource))
        //        {
        //            myCon.Open();
        //            using (SqlCommand myCommand = new SqlCommand(queryCheckUser, myCon))
        //            {
        //                myCommand.Parameters.AddWithValue("@username", userModel.UserName);

        //                userCount = (int)myCommand.ExecuteScalar();
        //            }
        //        }

        //        if (userCount > 0)
        //        {
        //            return new JsonResult(new { status = "failure", message = "Username already exists" });
        //        }

        //        // Nếu không tồn tại, thêm người dùng vào cơ sở dữ liệu
        //        string queryInsertUser = "INSERT INTO Users (username,email, password) VALUES (@username,@email, @password)";

        //        using (SqlConnection myCon = new SqlConnection(sqlDataSource))
        //        {
        //            myCon.Open();
        //            using (SqlCommand myCommand = new SqlCommand(queryInsertUser, myCon))
        //            {
        //                myCommand.Parameters.AddWithValue("@username", userModel.UserName);
        //                myCommand.Parameters.AddWithValue("@email", userModel.Email);
        //                myCommand.Parameters.AddWithValue("@password", userModel.Password);  // Lưu mật khẩu trực tiếp (nên mã hóa mật khẩu trong thực tế)

        //                int result = myCommand.ExecuteNonQuery();
        //                if (result > 0)
        //                {
        //                    return new JsonResult(new { status = "success", message = "Registration successful" });
        //                }
        //                else
        //                {
        //                    return new JsonResult(new { status = "failure", message = "An error occurred while registering" });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return new JsonResult(new { status = "error", message = $"An error occurred: {ex.Message}" });
        //    }
        //}

    }
}


// Phương thức đăng ký (POST)


//        {
//            String query = "select count(*) from Users where username = '" + userModel.UserName + "' and password = '" + userModel.Password + "'";
//            DataTable table = new DataTable();
//            String sqlDataSource = _configuration.GetConnectionString("WBMT");
//            SqlDataReader myReader;
//            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
//            {
//                myCon.Open();
//                using (SqlCommand myCommand = new SqlCommand(query, myCon))
//                {
//                    myReader = myCommand.ExecuteReader();
//                    table.Load(myReader);
//                    myReader.Close();
//                    myCon.Close();
//                }
//            }
//            return new JsonResult(table);
//        }
//    }
//}


