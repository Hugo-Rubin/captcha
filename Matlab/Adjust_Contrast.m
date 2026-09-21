function [ adj_img, adj_imgInv ] = Adjust_Contrast( img )
%ADJUST_CONTRAST Summary of this function goes here
%   Detailed explanation goes here

if ndims(img) == 3
    gs_img = Grayscale(img);
else
    gs_img = img;
end

adj_img = imadjust(gs_img, stretchlim(gs_img, 0), []);
%adj_img = imcontrast(image);

adj_imgInv = imadjust(gs_img, stretchlim(gs_img, 0), [1 0]);

imshow(gs_img), figure, imshow(adj_img), title('High Contrast');
figure, imshow(adj_imgInv), title('High Contrast (inverted)');


end

