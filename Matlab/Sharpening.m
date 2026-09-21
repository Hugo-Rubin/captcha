function [ sharpImg ] = Sharpening( img )
%SHARPENING Summary of this function goes here
%   Detailed explanation goes here

subplot(2,2,1), imshow(img), title('Original Image');

gaussianFilter = [1,4,7,4,1;4,20,33,20,4;7,33,55,33,7;4,20,33,20,4;1,4,7,4,1];

gaussianFilter = gaussianFilter / sum(sum(gaussianFilter));

gaussianImg = imfilter(img, gaussianFilter);
subplot(2,2,2), imshow(gaussianImg), title('Gaussian Image');

edgeFilter = [-1,-1,-1;-1,8,-1;-1,-1,-1];
edgeImg = imfilter(gaussianImg, edgeFilter);
subplot(2,2,3), imshow(edgeImg), title('Edges from Blurred Image');

sharpImg = gaussianImg + edgeImg;
subplot(2,2,4), imshow(sharpImg), title('Sharp Image (sharper than original)');


end

