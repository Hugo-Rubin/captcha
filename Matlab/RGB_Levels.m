function [  ] = RGB_Levels( img )
%RGB_LEVELS Summary of this function goes here
%   Detailed explanation goes here

subplot(2,2,1), image(img), title('Image (RGB)');
subplot(2,2,2), imshow(img(:,:,1)), title('Intensity Image: Red Layer');
subplot(2,2,3), imshow(img(:,:,2)), title('Intensity Image: Green Layer');
subplot(2,2,4), imshow(img(:,:,3)), title('Intensity Image: Blue Layer');


end

