function  I2 = imunwarp( I, polynomial )
%IMUNWARP Shifts the image per column vertically by polynomial value
%   Offset provided by polynomial value is negated to undo the original shift

[nrows, ncols, nbands] = size(I);

if nbands > 1
    I = rgb2gray(I);
end

x = 1:ncols;

offsets = polyval(polynomial, x);

I2 = 255 * ones(nrows, ncols, 'uint8');
for c=x
    offset = -round(offsets(c));
    if offset > 0
        o = nrows - offset + 1;
        I2(offset:end,c) = I(1:o,c);
    else
        I2(1:end+offset,c) = I(-offset+1:end,c);
    end
end
end

