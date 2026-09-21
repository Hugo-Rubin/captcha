function [ edgesAll ] = RGB_Edge( img )
%RGB_Edge Summary of this function goes here
%   Detailed explanation goes here

smallImg = img(1:4:size(img,1), 1:4:size(img,2), :);
subplot(2,2,1), imshow(smallImg), title('Resized Image (for viewing purposes only)');

edges = [];
edges(:,:,1) = edge(smallImg(:,:,1), 'sobel');
edges(:,:,2) = edge(smallImg(:,:,2), 'sobel');
edges(:,:,3) = edge(smallImg(:,:,3), 'sobel');
subplot(2,2,2), imshow(edges(:,:,1)), title('Image Edges Channel R');
subplot(2,2,3), imshow(edges(:,:,2)), title('Image Edges Channel G');
subplot(2,2,4), imshow(edges(:,:,3)), title('Image Edges Channel B');

figure, 
subplot(1,2,1), imshow(edges), title('Img Edges RGB');
edgesAll = edges(:,:,1) + edges(:,:,2) + edges(:,:,3);
subplot(1,2,2), imshow(edgesAll), title('Combined Edges in One Intensity Image');


end

