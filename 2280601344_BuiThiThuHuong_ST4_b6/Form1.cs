using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _2280601344_BuiThiThuHuong_ST4_b6.Models;
using System.Data.Entity;

namespace _2280601344_BuiThiThuHuong_ST4_b6
{
    public partial class Form1 : Form
    {
        Model1 db = new Model1();
        private void LoadSinhVien()
        {
            dtgSinhVien.DataSource = (from student in db.Students
                                      join faculity in db.Faculties on student.FacultyID equals faculity.FacultyID
                                      select new
                                      {
                                          student.StudentID,
                                          student.FullName,
                                          student.AverageScore,
                                          FacultyName = faculity.FacultyName
                                      }).ToList();
        }
        private void LoadKhoa()
        {
            var faculity = db.Faculties.ToList();
            cbBKhoa.DataSource = faculity;
            cbBKhoa.DisplayMember = "FacultyName";
            cbBKhoa.ValueMember = "FacultyID";
            cbBKhoa.SelectedIndex = -1;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            LoadSinhVien();
            LoadKhoa();

        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoten.Text) ||
                string.IsNullOrWhiteSpace(txtDtb.Text)) 
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbBKhoa.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn khoa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return;
            }
            if (float.TryParse(txtDtb.Text, out float dtb))
            {
                if (dtb < 0 || dtb > 10)
                {
                    MessageBox.Show("Điểm trung bình phải nằm trong khoảng từ 0 đến 10.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Điểm trung bình không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string studentId = txtMaSV.Text;
            var existingStudent = db.Students.FirstOrDefault(s => s.StudentID == studentId);
            if (existingStudent != null)
            {
                MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã số sinh viên khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? facultyId = cbBKhoa.SelectedValue as int?;
            
            Student student = new Student
            {
                StudentID = txtMaSV.Text,
                FullName = txtHoten.Text,
                AverageScore = float.Parse(txtDtb.Text),
                FacultyID = facultyId
            };
            db.Students.Add(student);
            db.SaveChanges();

            MessageBox.Show("Thêm mới dữ liệu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadSinhVien();
            ClearDuLieu();
        }
        private void ClearDuLieu()
        {
            txtMaSV.Text = "";
            txtHoten.Text = "";
            cbBKhoa.SelectedIndex = -1;
            txtDtb.Text = "";
        }

        private void btnSua_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoten.Text) ||
                string.IsNullOrWhiteSpace(txtDtb.Text) )
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbBKhoa.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn khoa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (float.TryParse(txtDtb.Text, out float dtb))
            {
                if (dtb < 0 || dtb > 10)
                {
                    MessageBox.Show("Điểm trung bình phải nằm trong khoảng từ 0 đến 10.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Điểm trung bình không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return;
            }
            // Tìm sinh viên trong cơ sở dữ liệu
            var student = db.Students.FirstOrDefault(s => s.StudentID == txtMaSV.Text);
            if (student != null)
            {
                // Cập nhật thông tin
                student.FullName = txtHoten.Text.Trim();

                student.AverageScore = float.Parse(txtDtb.Text);

                int? facultyId = cbBKhoa.SelectedValue as int?;
                student.FacultyID = facultyId;
                // Lưu thay đổi
                db.SaveChanges();

                MessageBox.Show("Cập nhật dữ liệu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại danh sách và làm trống các ô nhập liệu
                LoadSinhVien();
                ClearDuLieu();
            }
            else
            {
                MessageBox.Show("Không tìm thấy MSSV cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dtgSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgSinhVien.SelectedRows.Count > 0)
            {
                var row = dtgSinhVien.SelectedRows[0];
                txtMaSV.Text = row.Cells[0].Value.ToString();
                txtHoten.Text = row.Cells[1].Value.ToString();
                txtDtb.Text = row.Cells[2].Value.ToString();
                string facultyName = row.Cells[3].Value.ToString();
                var faculty = db.Faculties.FirstOrDefault(f => f.FacultyName == facultyName);
                if (faculty != null)
                {
                    cbBKhoa.SelectedValue = faculty.FacultyID;
                }
                else
                {
                    cbBKhoa.SelectedIndex = -1; // Không tìm thấy khoa
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dtgSinhVien.SelectedRows.Count > 0)
            {
                // Lấy mã sinh viên từ hàng được chọn
                var row = dtgSinhVien.SelectedRows[0];
                string studentId = row.Cells["STUDENTID"].Value?.ToString();

                if (!string.IsNullOrEmpty(studentId))
                {
                    // Tìm sinh viên trong database theo STUDENTID
                    var student = db.Students.FirstOrDefault(s => s.StudentID == studentId);
                    if (student != null)
                    {
                        // Xác nhận trước khi xóa
                        var result = MessageBox.Show($"Bạn có chắc muốn xóa sinh viên với mã: {studentId}?",
                                                     "Xác nhận",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            // Xóa sinh viên khỏi database
                            db.Students.Remove(student);
                            db.SaveChanges();

                            // Thông báo và cập nhật lại danh sách
                            MessageBox.Show("Xóa sinh viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSinhVien();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Không có mã sinh viên hợp lệ được chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       
    }
}
