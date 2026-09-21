function [ C1 ] = Corners( img )
%CORNERS Summary of this function goes here
%   Detailed explanation goes here

if ndims(img) == 3
    gs_img = Grayscale(img);
else
    gs_img = img;
end

C1 = corner(gs_img);
C2 = corner(gs_img, 'MinimumEigenvalue');

figure,
imshow(gs_img);
hold on
plot(C1(:,1), C1(:,2), 'go'), title('Harris Corner Detection');

figure,
imshow(gs_img);
hold on
plot(C2(:,1), C2(:,2), 'cs'), title('Minimum Eigenvalue Corner Detection');

end