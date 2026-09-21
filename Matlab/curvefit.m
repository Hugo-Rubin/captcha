function p = curvefit( I, order, trim )
%CURVEFIT Fit horizontal polynomial curve based on midpoints of the image
%   Vertical midpoints automatically determined by region of black pixels
%   at the top and bottom.  Applies polynomial fit of the specified order
%   to the midpoints.  Midpoints are trimmed by duplicating edge regions
%   to prevent extremities from affecting the fit.

if size(I, 3) > 1
    I2 = rgb2gray(I);
else
    I2 = I;
    I = repmat(I, [1 1 3]);
end

[nrows, ncols] = size(I2);
x = (1:ncols)';

rMinIdx = zeros(ncols, 1);
rMaxIdx = zeros(ncols, 1);

lastBGIdx = 1;
lastFGIdx = 1;

% Find midpoints
for c=1:ncols
    for r=1:nrows
        if (I2(r,c) ~= 255)
            break;
        end
        rMinIdx(c) = r;
    end

    for r=nrows:-1:1
        if (I2(r,c) ~= 255)
            break;
        end
        rMaxIdx(c) = r;
    end

    % Boundaries for absolute top and bottom row indices between black pixels
    if rMinIdx(c) < rMaxIdx(c)
        lastFGIdx = c;
    elseif lastBGIdx >= lastFGIdx
        lastBGIdx = c;
    end
end

rMidIdx = floor((rMinIdx + rMaxIdx)/2);

[rAbsMin, ~] = min(rMinIdx);
[rAbsMax, ~] = max(rMaxIdx);

rCenter = floor((rAbsMin + rAbsMax)/2);

% Trim by duplication to avoid protruding corners of warped letters
xRegion = (lastBGIdx:lastFGIdx)';
y = rMidIdx(lastBGIdx:lastFGIdx);
y(1:trim) = y(trim);
y(end-trim:end) = y(end-trim);
rMidIdx(lastBGIdx:lastFGIdx) = y;

% Apply polynomial fitting, and enumerate values to show on plot
% Only interested on offset from vertical center, thus use y-rCenter
p = polyfit(xRegion, y-rCenter, order);
f = polyval(p, x) + rCenter;

% Show image
imshow(I);

% Plots lines on top of the image
hold on

% Midpoints, red
plot(x, rMidIdx, 'r');

% Fitted polynomial curve, blue
plot(x, f, 'b');
hold off

end

