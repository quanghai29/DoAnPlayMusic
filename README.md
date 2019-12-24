# DoAnPlayMusic
link github: https://github.com/quanghai29/DoAnPlayMusic.git

Tổng quan giao diên, vùng làm việc :
+ Thanh trên cùng là các button chức năng
+ Vùng màu trắng hiển thị danh sách bài hát nút xóa , thêm bài hát
+ Vùng màu tím để hiển thị tên các play list mà người dùng đã lưu
+ Vùng màu đỏ để hiển thị thanh chơi nhạc , play pause,... 

Design partern sử dụng trong đồ án:Facade  
Tham khảo link: https://javadesign-patterns.blogspot.com/p/facade-design-pattern.html 

Phân chia công việc:
-  Duy: 
+ Chọn một tập tin để play
+ Chơi playlist theo chế độ tuần tự hoặc ngẫu nhiên 
+ Chơi playlist theo chế độ lặp 1 lần hoặc lặp vô tận 
+ Chấm dứt việc chơi một tập tin 
+ Hiển thị progress của tập tin đang chơi (từ 0:0 đến tổng thời gian của tập tin, ví dụ 5:30)
- Đồng: 
+ Xóa một hoặc nhiều bài hát trong playlist
+ Lưu danh sách playlist
+ Nạp lại playlist từ tập tin đã lưu (1 điểm)
+ Lưu và nạp lại các thông tin sau khi chương trình chạy lên:
  -	Danh sách playlist chơi lần cuối
  -	Tập tin của playlist lần cuối đang chơi để người dùng bắt đầu chơi tiếp
- Hải :
+ Tinh chỉnh giao diện
+ Hook bàn phím
+ Tạo ra playlist trống và thêm tập tin đa phương tiện vào 
+ Set volume , set animation đĩa quay (nếu còn thời gian)
