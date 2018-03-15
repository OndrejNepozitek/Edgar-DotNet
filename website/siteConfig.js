/**
 * Copyright (c) 2017-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

const siteConfig = {
  title: 'Procedural layout generation' /* title for your website */,
  tagline: 'Library for procedural generation of 2D layouts based on a graph of room connections.',
  url: 'https://ondrejnepozitek.github.io/' /* your website url */,
  baseUrl: '/MapGeneration/' /* base url for your project */,
  projectName: 'MapGeneration',
  headerLinks: [
    {doc: 'introduction', label: 'Docs'},
    {page: 'help', label: 'Help'},
    {blog: true, label: 'Blog'},
  ],
  /* path to images for header/footer */
  /*headerIcon: 'img/docusaurus.svg',*/
  /*footerIcon: 'img/docusaurus.svg',
  favicon: 'img/favicon.png',*/
  /* colors for website */
  colors: {
    primaryColor: '#db4d3f',
    secondaryColor: '#e0e0e0',
  },
  /* custom fonts for website */
  /*fonts: {
    myFont: [
      "Times New Roman",
      "Serif"
    ],
    myOtherFont: [
      "-apple-system",
      "system-ui"
    ]
  },*/
  // This copyright info is used in /core/Footer.js and blog rss/atom feeds.
  users: [],
  copyright:
    'Copyright © ' +
    new Date().getFullYear() +
    ' Your Name or Your Company Name',
  organizationName: 'OndrejNepozitek', // or set an env variable ORGANIZATION_NAME
  projectName: 'MapGeneration', // or set an env variable PROJECT_NAME
  highlight: {
    // Highlight.js theme to use for syntax highlighting in code blocks
    theme: 'default',
  },
  scripts: ['https://buttons.github.io/buttons.js'],
  // You may provide arbitrary config keys to be used as needed by your template.
  repoUrl: 'https://github.com/facebook/test-site',
};

module.exports = siteConfig;
