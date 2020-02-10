import React from "react";
import useBaseUrl from "@docusaurus/useBaseUrl";

const gutter = 2;
const Image = props => (
  <div
    style={{
      display: "inline-block",
      margin: gutter,
      overflow: "hidden",
      position: "relative",
      width: `calc(${100 / props.cols}% - ${gutter * 2}px)`
    }}
  >
    {props.children}
  </div>
);

export const Gallery = props => (
  <div style={{ fontSize: "0px" }}>
    {React.Children.map(props.children, child =>
      React.cloneElement(child, { cols: props.cols || 4 })
    )}
  </div>
);

export const GalleryImage = props => (
  <a href={useBaseUrl(props.src)} target="_blank">
    <Image cols={props.cols}>
      <img src={useBaseUrl(props.src)} alt="result" />
    </Image>
  </a>
);
