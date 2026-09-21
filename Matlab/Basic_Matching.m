function [ matches, scores ] = Basic_Matching( img1, img2 )
%BASIC_MATCHING Summary of this function goes here
%   Detailed explanation goes here

if ndims(img1) == 3
    gs1 = Grayscale(img1);
else
    gs1 = img1;
end

if ndims(img2) == 3
    gs2 = Grayscale(img2);
else
    gs2 = img2;
end

I1 = single(gs1);
I2 = single(gs2);

[f1, d1] = vl_sift(I1);
[f2, d2] = vl_sift(I2);

%imshow(gs1);
%hold on;
%perm = randperm(size(f1,2)); 
%sel = perm(1:size(f1,2));
%h1 = vl_plotframe(f1(:,sel)); 
%h2 = vl_plotframe(f1(:,sel)); 
%set(h1, 'color', 'm', 'linewidth', 3);
%set(h2, 'color', 'g', 'linewidth', 2);

%pause;

%figure,
%imshow(gs2);
%hold on;
%perm = randperm(size(f2,2)); 
%sel = perm(1:50);
%h3 = vl_plotframe(f2(:,sel)); 
%h4 = vl_plotframe(f2(:,sel)); 
%set(h3, 'color', 'm', 'linewidth', 3);
%set(h4, 'color', 'g', 'linewidth', 2);

%pause;

[matches, scores] = vl_ubcmatch(d1, d2, 2) ;

figure,
subplot(1,2,1); 
imshow(uint8(gs1)); 
hold on; 
plot(f1(1, matches(1,:)), f1(2, matches(1,:)), 'bo'), title('Matching Features in Image 1'); 
 
subplot(1,2,2); 
imshow(uint8(gs2)); 
hold on;
plot(f2(1, matches(2,:)), f2(2, matches(2,:)), 'ms'), title('Matching Features in Image 2'); 


end

