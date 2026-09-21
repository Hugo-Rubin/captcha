img = imread('t.png');
imshow(img);
pause;

gs_img = Grayscale(img);
imshow(gs_img);
pause;

gaussian = Gaussian(gs_img);
pause;

adjusted_img = Adjust_Contrast(gaussian);
pause;

V = Vertical_ED(adjusted_img);
pause;
H = Horizontal_ED(adjusted_img);
pause;
gradient = Gradient_Img(V, H);
pause;

canny = Canny_Edge(adjusted_img);
pause;

corners = Corners(adjusted_img);
pause;

[features, desc] = Sift(adjusted_img);
pause;

match1 = imread('teste1.jpg');
match2 = imread('teste2.jpg');

[matches, scores] = Basic_Matching(match1, match2);