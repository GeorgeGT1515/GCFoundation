import gulp from 'gulp';
import dartSass from 'sass';
import gulpSass from 'gulp-sass';
import cleanCSS from 'gulp-clean-css';
import sourcemaps from 'gulp-sourcemaps';
import uglify from 'gulp-uglify';
import concat from 'gulp-concat';
import babel from 'gulp-babel';
import rename from 'gulp-rename';
import autoprefixer from 'gulp-autoprefixer';
import newer from 'gulp-newer';
import { deleteAsync } from 'del';
import plumber from 'gulp-plumber';
import notify from 'gulp-notify';
import through2 from 'through2';

const { src, dest, watch, series, parallel } = gulp;
const sass = gulpSass(dartSass);

// Development mode flag
const isDev = process.env.NODE_ENV === 'development';

// Paths
const paths = {
    scss: {
        // Only process main SCSS files (not partials)
        src: 'wwwroot/src/scss/**/*.scss',
        dest: 'wwwroot/css'
    },
    js: {
        src: 'wwwroot/src/js/**/*.js',
        dest: 'wwwroot/js'
    },
    vendor: {
        gridjs: {
            js: 'node_modules/gridjs/dist/*.min.js',
            css: 'node_modules/gridjs/dist/theme/*.min.css',
            dest: 'wwwroot/lib/gridjs',
            cssDest: 'wwwroot/lib/gridjs/theme'
        }
    }
};

// Error handling
const errorHandler = {
    errorHandler: notify.onError({
        title: 'Error',
        message: '<%= error.message %>'
    })
};

// Clean generated files
async function clean() {
    await deleteAsync([
        paths.scss.dest + '/*',
        paths.js.dest + '/*'
    ]);
}

// Compile SCSS
function styles() {
    return src(paths.scss.src)
        .pipe(plumber(errorHandler))
        .pipe(newer({ dest: paths.scss.dest, ext: '.min.css' }))
        .pipe(sourcemaps.init())
        .pipe(sass({
            includePaths: ['wwwroot/src/scss'],
            outputStyle: 'compressed'
        }).on('error', sass.logError))
        .pipe(autoprefixer({ cascade: false }))
        .pipe(cleanCSS())
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.write('.'))
        .pipe(dest(paths.scss.dest));
}

// Bundle and minify JS
function scripts() {
    return src(paths.js.src)
        .pipe(plumber(errorHandler))
        .pipe(newer({ dest: paths.js.dest, ext: '.min.js' }))
        .pipe(sourcemaps.init())
        .pipe(babel({
            presets: ['@babel/preset-env']
        }))
        .pipe(concat('foundation.js'))
        .pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
        .pipe(sourcemaps.write('.'))
        .pipe(dest(paths.js.dest));
}

// Copy Grid.js vendor assets
function copyGridjsJs() {
    return src(paths.vendor.gridjs.js)
        .pipe(plumber(errorHandler))
        .pipe(newer(paths.vendor.gridjs.dest))
        .pipe(dest(paths.vendor.gridjs.dest));
}

function copyGridjsCss() {
    return src(paths.vendor.gridjs.css)
        .pipe(plumber(errorHandler))
        .pipe(newer(paths.vendor.gridjs.cssDest))
        .pipe(dest(paths.vendor.gridjs.cssDest));
}

// Build tasks
export const build = series(
    clean,
    parallel(styles, scripts, copyGridjsJs, copyGridjsCss)
);

// Watch task (run separately with 'gulp watch')
export const watchTask = function() {
    // Initial build
    build();
    
    // Then watch for changes
    watch(paths.scss.src, styles);
    watch(paths.js.src, scripts);
};

// Default task
export default build;

function noop() {
    return through2.obj();
}