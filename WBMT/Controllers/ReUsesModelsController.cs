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
    public class ReUsersModelsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReUsersModelsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //DK
        [HttpPost("dangky")]
        public JsonResult Register([FromBody] ReUsersModel reUsersModel)
        {
            try
            {
                // Kiểm tra nếu mật khẩu và xác nhận mật khẩu không khớp
                if (reUsersModel.Password != reUsersModel.ConfirmPassword)
                {
                    return new JsonResult(new { status = "failure", message = "Passwords do not match" });
                }

                // Kiểm tra nếu tên người dùng đã tồn tại
                string queryCheckUser = "SELECT COUNT(*) AS UserCount FROM Users WHERE username = @username";
                int userCount = 0;
                string sqlDataSource = _configuration.GetConnectionString("WBMT");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(queryCheckUser, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", reUsersModel.UserName);

                        userCount = (int)myCommand.ExecuteScalar();
                    }
                }

                if (userCount > 0)
                {
                    return new JsonResult(new { status = "failure", message = "Username already exists" });
                }

                // Nếu không tồn tại, thêm người dùng vào cơ sở dữ liệu
                string queryInsertUsers = "INSERT INTO Users (username,email, password) VALUES (@username,@email, @password)";

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(queryInsertUsers, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@username", reUsersModel.UserName);
                        myCommand.Parameters.AddWithValue("@email", reUsersModel.Email);
                        myCommand.Parameters.AddWithValue("@password", reUsersModel.Password);  // Lưu mật khẩu trực tiếp (nên mã hóa mật khẩu trong thực tế)

                        int result = myCommand.ExecuteNonQuery();
                        if (result > 0)
                        {
                            return new JsonResult(new { status = "success", message = "Registration successful" });
                        }
                        else
                        {
                            return new JsonResult(new { status = "failure", message = "An error occurred while registering" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { status = "error", message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}