function [ ] = Edge_Examples( img )
%EDGE_EXAMPLES Summary of this function goes here
%   Detailed explanation goes here

horizontalKernel = [-1,-1,-1;2,2,2;-1,-1,-1];
verticalKernel = [-1,2,-1;-1,2,-1;-1,2,-1];
diagUpKernel = [-1,-1,2;-1,2,-1;2,-1,-1];
diagDownKernel = [2,-1,-1;-1,2,-1;-1,-1,2];

figure;
horizontalImg = imfilter(img, horizontalKernel);
subplot(2,2,1), imshow(horizontalImg), title('Horizontal lines');
verticalimg = imfilter(img, verticalKernel);
subplot(2,2,2), imshow(verticalimg), title('Vertical lines');
diagDownimg = imfilter(img, diagDownKernel);
subplot(2,2,3), imshow(diagDownimg), title('Diagonals down slope');
diagUpimg = imfilter(img, diagUpKernel);
subplot(2,2,4), imshow(diagUpimg), title('Diagonals up slope');

end

