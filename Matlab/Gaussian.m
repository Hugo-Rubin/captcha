function [ gaussianImg ] = Gaussian( img )
%SMOOTHING Summary of this function goes here
%   Detailed explanation goes here

gaussianFilter = fspecial('gaussian', [7, 7], 5); % blur matrix size = 7

gaussianImg = imfilter(img, gaussianFilter, 'symmetric', 'conv');
subplot(1,2,1), image(img), title('Original Image');
subplot(1,2,2), title('Gaussian Blur Applied in the Image'), image(gaussianImg);

if ndims(img) == 3
    figure,
    gaussianR = filter2(gaussianFilter, img(:,:,1));
    subplot(1,2,1), imshow(img(:,:,1)), title('Red Layer of Original Image');
    subplot(1,2,2), imshow(uint8(gaussianR)), title('Gaussian Blur in the Red Layer');
    gaussianG = filter2(gaussianFilter, img(:,:,2));
    subplot(1,2,3), imshow(img(:,:,2)), title('Green Layer of Original Image');
    subplot(1,2,4), imshow(uint8(gaussianG)), title('Gaussian Blur in the Green Layer');
    gaussianB = filter2(gaussianFilter, img(:,:,3));
    subplot(1,2,5), imshow(img(:,:,3)), title('Blue Layer of Original Image');
    subplot(1,2,6), imshow(uint8(gaussianB)), title('Gaussian Blur in the Blue Layer');
end

end

