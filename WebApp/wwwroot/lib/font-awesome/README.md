# Font awesome free version V6

- [Synopsis](#synopsis)
- [Install](#install)
- [Usage](#usage)
- [Example](#example)

## Synopsis

By [©Fonticons, Inc.][company]

Font Awesome Free is free, open source, and GPL friendly. You can use it for commercial projects, open source projects, or really almost whatever you want.

## Install

Install the fontface via npm.

```bash
npm install --save @easyfonts/font-awesome-free
## OR
yarn add @easyfonts/font-awesome-free
```

## Usage

Include the font in your build with (tailwind css will look in node_modules as well)

```css
@import "@easyfonts/fontawesome-free";
```

Reference in your css

```css
.someclass {
  font-family: "Font Awesome 6 Free";
}
```

With above css, the tag below will now show the glyph in 700 weight

```html
<i class="fa fa-calendar-alt" />
```

This will install all fontfiles in node modules (4.4Mb).

If you are not using all font weights you can select a subset of fontfiles to be included into your build.
You build will be consequently smaller

| import statement                                    | prefix-class | description                          |
| --------------------------------------------------- | ------------ | ------------------------------------ |
| `import '@easyfonts/fontawesome-free/brands.css';`  | `fabs`       | glyphs of well known brands          |
| `import '@easyfonts/fontawesome-free/regular.css';` | `fa`         | regular glyphs                       |
| `import '@easyfonts/fontawesome-free/solid.css';`   | `fas`        | filled up glyphs and thicker strokes |

## Example

Include only extra-bold in your bundle

css:

```css
@import "@easyfonts/fontawesome-free/solid.css";
```

javascript: (webpack, vite)

```javascript
import "@easyfonts/fontawesome-free/solid.css";
```


## Glyph CheatSheet.

![Font Awesome v6 part 1][sample1]
![Font Awesome v6 part 2][sample2]
![Font Awesome v6 part 3][sample3]
![Font Awesome v6 part 4][sample4]

[sample1]: ./images/fa-sc-1.png
[sample2]: ./images/fa-sc-2.png
[sample3]: ./images/fa-sc-3.png
[sample4]: ./images/fa-sc-4.png
[license]: https://fontawesome.com/license/free
[company]: https://fontawesome.com/versions
