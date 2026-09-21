function [ gs_img ] = Grayscale_Multi( image )
%TEST Summary of this function goes here
%   Detailed explanation goes here

[img, map] = imread( image );

%imshow(Img,map);

if isempty(map) && size(img, 3) == 3
    gs_img = rgb2gray(img);
elseif ~isempty(map)
    gs_img = ind2gray(img, map);
end

end

